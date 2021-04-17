import React, { Component, Fragment } from 'react';
import { formatMessage, FormattedMessage } from 'umi/locale';
import { List } from 'antd';
import { connect } from 'dva';
import { Popconfirm } from 'antd';
import { message, Modal, Select, Input, Form, Row, Col, Button, Popover, Progress } from 'antd';
import Link from 'umi/link';
import router from 'umi/router';
import styles from './SecurityView.less';
import { thisExpression } from '@babel/types';

const { Option } = Select;
const InputGroup = Input.Group;
const FormItem = Form.Item;

const notVerify = <font style={{ marginLeft: 5, }} className="weak">未验证</font>;


const passwordStatusMap = {
  ok: (
    <div className={styles.success}>
      <FormattedMessage id="validation.password.strength.strong" />
    </div>
  ),
  pass: (
    <div className={styles.warning}>
      <FormattedMessage id="validation.password.strength.medium" />
    </div>
  ),
  poor: (
    <div className={styles.error}>
      <FormattedMessage id="validation.password.strength.short" />
    </div>
  ),
};

const passwordProgressMap = {
  ok: 'success',
  pass: 'normal',
  poor: 'exception',
};

@connect(({ user }) => ({
  currentUser: user.currentUser,
}))
@Form.create()
class SecurityView extends Component {
  state = {
    // 设置绑定新手机
    visibleBindPhone: false,
    count: 0,
    newPhone: '',
    bindNewPhoneCode: '',

    // 设置绑定新邮箱
    visibleBindEmail: false,
    newEmail: '',

    // 修改密码
    visiblePassword: false,
    confirmDirty: false,
    visible: false,
    help: '',
  };

  handleSubmitPassword = () => {
    const { form, dispatch } = this.props;
    form.validateFields({ force: true }, (err, values) => {
      if (err) {
        return;
      }
      new Promise(resolve => {
        dispatch({
          type: 'user/changePassword',
          payload: {
            resolve,
            params: {
              // ...values,
              oldPassword: values['oldPassword'],
              confirmPassword: values['confirm'],
              newPassword: values['password']
            }
          },
        });
      }).then(res => {
        if (res.success === true) {
          this.setState({ visiblePassword: false });
          message.info("密码修改成功,请重新登录");
          dispatch({
            type: 'login/logout',
          });
          // setTimeout(() => {
          //   router.go(0);
          // }, 1000);
        } else {
          message.error(res.message);
        }
      });
    });
  };

  handleVisibleBindPhone = (e) => {
    this.setState({ visibleBindPhone: e });
  }

  onGetCaptcha = () => {
    const { dispatch } = this.props;
    if (!this.state.newPhone) {
      message.error('请输入绑定的手机号');
      return;
    }
    let count = 59;
    this.setState({ count });
    this.interval = setInterval(() => {
      count -= 1;
      this.setState({ count });
      if (count === 0) {
        clearInterval(this.interval);
      }
    }, 1000);

    new Promise(resolve => {
      dispatch({
        type: 'user/addPhoneGetCaptcha',
        payload: {
          resolve,
          params: { phone: this.state.newPhone }
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("发送成功");
      } else {
        message.warning(res.message);
      }
    });
  };

  onAddPhone = () => {
    const { dispatch } = this.props;
    if (!this.state.newPhone) {
      message.error('请输入绑定的手机号');
      return;
    }
    if (!this.state.bindNewPhoneCode) {
      message.error('请输入验证码');
      return;
    }

    new Promise(resolve => {
      dispatch({
        type: 'user/addPhone',
        payload: {
          resolve,
          params: {
            phone: this.state.newPhone,
            code: this.state.bindNewPhoneCode
          }
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("绑定成功");
        this.handleVisibleBindPhone(false);
      } else {
        message.warning(res.message);
      }
    });
  };

  handleRemoveEmail = () => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'user/removeEmail',
        payload: {
          resolve,
          params: {}
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("移除绑定成功");
      } else {
        message.error(res.message);
      }
    });
  };

  handleVisibleBindEmail = (e) => {
    this.setState({ visibleBindEmail: e });
  }

  onAddEmail = () => {
    const { dispatch } = this.props;
    if (!this.state.newEmail) {
      message.error('请输入绑定的邮箱地址');
      return;
    }

    new Promise(resolve => {
      dispatch({
        type: 'user/addEmailSendToken',
        payload: {
          resolve,
          params: {
            email: this.state.newEmail,
          }
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("绑定邮箱邮件发送成功,请登录邮箱查看");
        this.handleVisibleBindEmail(false);
      } else {
        message.warning(res.message);
      }
    });
  };

  onSendConfirmEmail = () => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'user/sendConfirmEmail',
        payload: {
          resolve,
          params: {}
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("验证邮件发送成功,请登录邮箱查看");
      } else {
        message.warning(res.message);
      }
    });
  };

  handleRemovePhone = () => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'user/removePhone',
        payload: {
          resolve,
          params: {}
        },
      });
    }).then(res => {
      if (res.success === true) {
        message.info("移除绑定成功");
      } else {
        message.error(res.message);
      }
    });
  };

  getData = () => {
    const { currentUser } = this.props;
    return [{
      title: formatMessage({ id: 'app.settings.security.password' }, {}),
      description: '已设置',
      actions: [
        <a onClick={() => { this.handleVisibleModifyPassword(true) }}>修改</a>
      ],
    },
    {
      // 注意，绑定的手机一定是已验证的
      // title: formatMessage({ id: 'app.settings.security.phone' }, {}),
      // description: `${formatMessage({ id: 'app.settings.security.phone-description' }, {})}${currentUser.phone}`,
      title: '绑定手机',
      description: (
        <Fragment>
          {
            currentUser.phone ?
              formatMessage({ id: 'app.settings.security.phone-description' }, {}) : '无'
          }
          {currentUser.phone}
        </Fragment>
      ),
      actions: this.getPhoneArray()
    },
    {
      title: `绑定邮箱`,
      description: (
        <Fragment>
          {
            currentUser.email ?
              formatMessage({ id: 'app.settings.security.email-description' }, {}) : '无'
          }
          {currentUser.email}
          {currentUser.emailConfirmed || !currentUser.email ? null : notVerify}
        </Fragment>
      ),
      actions: this.getEmailArray()
    },
    ]
  };

  getPhoneArray = () => {
    const { currentUser } = this.props;
    let acs = [];
    if (currentUser.phone) {
      acs.push(<Popconfirm title="确定要移除此绑定吗?" onConfirm={() => this.handleRemovePhone()}>
        <a>删除</a>
      </Popconfirm>);
    } else {
      acs.push(<a onClick={() => { this.handleVisibleBindPhone(true) }}>设置</a>);
    }
    return acs;
  }

  getEmailArray = () => {
    const { currentUser } = this.props;
    let acs = [];
    if (currentUser.email) {
      if (!currentUser.emailConfirmed) {
        acs.push(<a onClick={this.onSendConfirmEmail}>立即验证</a>);
      }
      acs.push(<Popconfirm title="确定要移除此绑定吗?" onConfirm={() => this.handleRemoveEmail()}>
        <a>删除</a>
      </Popconfirm>);
    } else {
      acs.push(<a onClick={() => { this.handleVisibleBindEmail(true) }}>设置</a>);
    }
    return acs;
  }

  getPasswordStatus = () => {
    const { form } = this.props;
    const value = form.getFieldValue('password');
    if (value && value.length > 9) {
      return 'ok';
    }
    if (value && value.length > 5) {
      return 'pass';
    }
    return 'poor';
  };

  handleConfirmBlur = e => {
    const { value } = e.target;
    const { confirmDirty } = this.state;
    this.setState({ confirmDirty: confirmDirty || !!value });
  };

  checkConfirm = (rule, value, callback) => {
    const { form } = this.props;
    if (value && value !== form.getFieldValue('password')) {
      callback(formatMessage({ id: 'validation.password.twice' }));
    } else {
      callback();
    }
  };

  checkPassword = (rule, value, callback) => {
    const { visible, confirmDirty } = this.state;
    if (!value) {
      this.setState({
        help: formatMessage({ id: 'validation.password.required' }),
        visible: !!value,
      });
      callback('error');
    } else {
      this.setState({
        help: '',
      });
      if (!visible) {
        this.setState({
          visible: !!value,
        });
      }
      if (value.length < 6) {
        callback('error');
      } else {
        const { form } = this.props;
        if (value && confirmDirty) {
          form.validateFields(['confirm'], { force: true });
        }
        callback();
      }
    }
  };

  handleVisibleModifyPassword = (e) => {
    this.setState({ visiblePassword: e });
  }

  renderPasswordProgress = () => {
    const { form } = this.props;
    const value = form.getFieldValue('password');
    const passwordStatus = this.getPasswordStatus();
    return value && value.length ? (
      <div className={styles[`progress-${passwordStatus}`]}>
        <Progress
          status={passwordProgressMap[passwordStatus]}
          className={styles.progress}
          strokeWidth={6}
          percent={value.length * 10 > 100 ? 100 : value.length * 10}
          showInfo={false}
        />
      </div>
    ) : null;
  };

  render() {
    const { count } = this.state;
    const { form } = this.props;
    const { getFieldDecorator } = form;
    const { help, visible } = this.state;
    return (
      <div>
        <Fragment>
          <List
            itemLayout="horizontal"
            dataSource={this.getData()}
            renderItem={item => (
              <List.Item actions={item.actions}>
                <List.Item.Meta title={item.title} description={item.description} />
              </List.Item>
            )}
          />
        </Fragment>
        <Modal
          title={`修改密码`}
          destroyOnClose
          centered
          visible={this.state.visiblePassword}
          onCancel={() => { this.handleVisibleModifyPassword(false) }}
          onOk={() => { this.handleSubmitPassword() }}
        >
          <Form onSubmit={this.handleSubmitPassword}>
            <FormItem>
              {getFieldDecorator('oldPassword', {
                rules: [
                  {
                    required: true,
                    message: '请输入旧密码',
                  },
                ],
              })(
                <Input
                  size="large"
                  type="password"
                  placeholder="旧密码"
                />
              )}
            </FormItem>
            <FormItem help={help}>
              <Popover
                getPopupContainer={node => node.parentNode}
                content={
                  <div style={{ padding: '4px 0' }}>
                    {passwordStatusMap[this.getPasswordStatus()]}
                    {this.renderPasswordProgress()}
                    <div style={{ marginTop: 10 }}>
                      <FormattedMessage id="validation.password.strength.msg" />
                    </div>
                  </div>
                }
                overlayStyle={{ width: 240 }}
                placement="right"
                visible={visible}
              >
                {getFieldDecorator('password', {
                  rules: [{ validator: this.checkPassword, },],
                })(
                  <Input
                    size="large"
                    type="password"
                    placeholder={formatMessage({ id: 'form.password.placeholder' })}
                  />
                )}
              </Popover>
            </FormItem>
            <FormItem>
              {getFieldDecorator('confirm', {
                rules: [
                  {
                    required: true,
                    message: formatMessage({ id: 'validation.confirm-password.required' }),
                  },
                  {
                    validator: this.checkConfirm,
                  },
                ],
              })(
                <Input
                  size="large"
                  type="password"
                  placeholder={formatMessage({ id: 'form.confirm-password.placeholder' })}
                />
              )}
            </FormItem>
          </Form>
        </Modal>
        <Modal
          title={`绑定手机`}
          destroyOnClose
          centered
          visible={this.state.visibleBindPhone}
          onCancel={() => { this.handleVisibleBindPhone(false) }}
          onOk={this.onAddPhone}>
          <Form>
            <FormItem>
              <InputGroup compact>
                <Select
                  size="large"
                  value={'86'}
                  style={{ width: '25%' }}>
                  <Option value="86">+86</Option>
                </Select>
                <Input
                  size="large"
                  style={{ width: '75%' }}
                  onChange={(e) => {
                    this.setState({ newPhone: e.target.value })
                  }}
                />
              </InputGroup>
            </FormItem>
            <FormItem>
              <Row gutter={8}>
                <Col span={16}>
                  <Input
                    size="large"
                    placeholder={formatMessage({ id: 'form.verification-code.placeholder' })}
                    onChange={(e) => {
                      this.setState({ bindNewPhoneCode: e.target.value });
                    }}
                  />
                </Col>
                <Col span={8}>
                  <Button
                    size="large"
                    disabled={count}
                    // className={styles.getCaptcha}
                    onClick={this.onGetCaptcha}
                  >
                    {count
                      ? `${count} s`
                      : formatMessage({ id: 'app.register.get-verification-code' })}
                  </Button>
                </Col>
              </Row>
            </FormItem>
          </Form>
        </Modal>
        <Modal
          title={`绑定邮箱`}
          destroyOnClose
          centered
          visible={this.state.visibleBindEmail}
          onCancel={() => { this.handleVisibleBindEmail(false) }}
          okText="发送邮件"
          onOk={this.onAddEmail}>
          <Form>
            <FormItem>
              <InputGroup compact>
                <Input
                  placeholder="邮箱地址"
                  size="large"
                  onChange={(e) => {
                    this.setState({ newEmail: e.target.value })
                  }}
                />
              </InputGroup>
            </FormItem>
          </Form>
        </Modal>
      </div>
    );
  }
}

export default SecurityView;
