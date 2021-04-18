import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import { Checkbox, Alert, Icon } from 'antd';
import Login from '@/components/Login';
import styles from './Login.less';
import { message, Modal, Select, Input, Form, Row, Col, Button } from 'antd';
import router from 'umi/router';

const { Tab, UserName, Password, Mobile, Captcha, Submit } = Login;
const { Option } = Select;
const InputGroup = Input.Group;
const FormItem = Form.Item;

@Form.create()
@connect(({ login, loading }) => ({
  login,
  submitting: loading.effects['login/login'],
}))
class LoginPage extends Component {
  state = {
    type: 'account',
    autoLogin: true,

    count: 0,
    confirmDirty: false,
    visible: false,
    help: '',
    prefix: undefined
  };

  componentDidMount() {
    this.changeFactor(false);
  }

  onTabChange = type => {
    this.setState({ type });
  };

  onGetCaptcha2 = () => {
    const { form: { validateFields }, dispatch } = this.props;
    if (!this.state.prefix) {
      message.error('请选择验证方式');
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
        type: 'login/getCaptchaTwoFactor',
        payload: {
          resolve,
          params: { selectedProvider: this.state.prefix }
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

  handleSubmitTwoFactor = e => {
    e.preventDefault();
    const { form, dispatch } = this.props;
    form.validateFields({ force: true }, (err, values) => {
      if (!err) {
        new Promise(resolve => {
          dispatch({
            type: 'login/loginTwoFactor',
            payload: {
              resolve,
              params: {
                selectedProvider: this.state.prefix,
                code: values['captcha']
              }
            },
          });
        });
      }
    });
  };

  onGetCaptcha = () => new Promise(resolve => {
    this.loginForm.validateFields(['phone'], {}, (err, values) => {
      if (err)
        return;
      const { dispatch } = this.props;
      dispatch({
        type: 'login/getCaptcha',
        payload: { phone: values.phone },
      }).then(resolve);
      // dispatch({
      //   type: 'login/getCaptcha',
      //   payload: {
      //     resolve,
      //     params: { phone: values.phone },
      //   },
      // }).then(res => {
      //   if (res.success === true) {
      //     message.info('发送成功');
      //     resolve();
      //   } else {
      //     message.warning(res.message);
      //   }
      // });
    });
  });

  handleSubmit = (err, values) => {
    const { type } = this.state;
    if (!err) {
      const { dispatch } = this.props;
      dispatch({
        type: 'login/loginAdmin',
        payload: {
          ...values,
          type,
          rememberMe: this.state.autoLogin
        },
      });
    }
  };

  changeAutoLogin = e => {
    this.setState({
      autoLogin: e.target.checked,
    });
  };

  changeFactor = (e) => {
    const { dispatch } = this.props;
    dispatch({
      type: 'login/factor',
      payload: e,
    });
  };

  changePrefix = value => {
    this.setState({
      prefix: value,
    });
  };

  renderMessage = content => (
    <Alert style={{ marginBottom: 24 }} message={content} type="error" showIcon />
  );

  forgotPassword = () => {
    router.push('/user/forgot-password')
  }

  render() {
    const { login, submitting, form } = this.props;
    const { type, autoLogin } = this.state;

    const { providers } = login;
    const { getFieldDecorator } = form;
    const { count, prefix, help, visible } = this.state;

    return (
      <div className={styles.main}>
        <Login
          defaultActiveKey={type}
          onTabChange={this.onTabChange}
          onSubmit={this.handleSubmit}
          ref={form => {
            this.loginForm = form;
          }}
        >
          <Tab key="account" tab={formatMessage({ id: 'app.login.tab-login-credentials' })}>
            {login.status === 'error' &&
              login.type === 'account' &&
              !submitting &&
              this.renderMessage(formatMessage({ id: 'app.login.message-invalid-credentials' }))}
            <UserName name="name" placeholder="用户名/邮箱/手机号" />
            <Password
              name="password"
              placeholder="密码"
              onPressEnter={() => this.loginForm.validateFields(this.handleSubmit)}
            />
          </Tab>
          <Tab key="mobile" tab={formatMessage({ id: 'app.login.tab-login-mobile' })}>
            {login.status === 'error' &&
              login.type === 'mobile' &&
              !submitting &&
              this.renderMessage(
                formatMessage({ id: 'app.login.message-invalid-verification-code' })
              )}
            <Mobile name="phone" placeholder="手机号" />
            <Captcha name="code" placeholder="验证码" countDown={60} onGetCaptcha={this.onGetCaptcha} />
          </Tab>
          <div>
            <Checkbox checked={autoLogin} onChange={this.changeAutoLogin}>
              <FormattedMessage id="app.login.remember-me" />
            </Checkbox>
            <a style={{ float: 'right' }} onClick={this.forgotPassword}>
              <FormattedMessage id="app.login.forgot-password" />
            </a>
          </div>
          <Submit loading={submitting}>
            <FormattedMessage id="app.login.login" />
          </Submit>
          <div className={styles.other}>
            <FormattedMessage id="app.login.sign-in-with" />
            <Icon type="alipay-circle" className={styles.icon} theme="outlined" />
            <Icon type="taobao-circle" className={styles.icon} theme="outlined" />
            <Icon type="weibo-circle" className={styles.icon} theme="outlined" />
            <Link className={styles.register} to="/user/register">
              <FormattedMessage id="app.login.signup" />
            </Link>
          </div>
        </Login>

        <Modal
          title={`进一步验证`}
          destroyOnClose
          centered
          visible={login.requiresTwoFactor}
          onCancel={() => { this.changeFactor(false) }}
          onOk={this.handleSubmitTwoFactor}
        >
          <Form onSubmit={this.handleSubmitTwoFactor}>
            <FormItem>
              <InputGroup compact>
                <Select
                  size="large"
                  value={prefix}
                  onChange={this.changePrefix}
                  style={{ width: '25%' }}
                  placeholder="验证方式"
                >
                  {providers.map(x => <Option value={x.key} key={x.key}>{x.key}</Option>)}
                </Select>
                <Input
                  disabled
                  size="large"
                  style={{ width: '75%' }}
                  value={
                    providers && prefix && providers.find(c => c.key == prefix) ?
                      providers.find(c => c.key == prefix).value : null
                  }
                />
              </InputGroup>
            </FormItem>
            <FormItem>
              <Row gutter={8}>
                <Col span={16}>
                  {getFieldDecorator('captcha', {
                    rules: [
                      {
                        required: true,
                        message: formatMessage({ id: 'validation.verification-code.required' }),
                      },
                    ],
                  })(
                    <Input
                      size="large"
                      placeholder={formatMessage({ id: 'form.verification-code.placeholder' })}
                    />
                  )}
                </Col>
                <Col span={8}>
                  <Button
                    size="large"
                    disabled={count}
                    className={styles.getCaptcha}
                    onClick={this.onGetCaptcha2}
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

      </div>
    );
  }
}

export default LoginPage;
