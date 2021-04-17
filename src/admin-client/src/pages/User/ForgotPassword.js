import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import router from 'umi/router';
import styles from './Register.less';
import Login from '@/components/Login';
import { message, Modal, Select, Input, Form, Row, Col, Button, Popover, Progress } from 'antd';

const { Option } = Select;
const InputGroup = Input.Group;
const FormItem = Form.Item;

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

@connect()
@Form.create()
class ForgotPassword extends Component {
    constructor(props) {
        super(props);
        this.state = {
            submitting: false,
            name: '',
            user: {},
            options: [],
            optionValue: '',
            status: 0, //找回密码按钮=0 手机找回确认按钮=1 发送邮箱按钮=2 

            count: 0,
            confirmDirty: false,
            visible: false,
            help: '',

            disabledEmail: false,
        };
    }

    componentDidMount() {
    }

    handleGetUser = () => {
        if (!this.state.name) {
            message.error('请输入用户名/邮箱/手机号');
            return;
        }
        const { dispatch } = this.props;
        this.setState({ submitting: true, options: [], optionValue: '', user: {}, status: 0 });
        new Promise(resolve => {
            dispatch({
                type: 'user/getForgotPassword',
                payload: {
                    resolve,
                    params: {
                        name: this.state.name
                    },
                },
            });
        }).then(res => {
            this.setState({ submitting: false });
            if (res.success === true) {
                let os = [];
                if (res.data.phone) {
                    os.push(<Option value="Phone" key="Phone">Phone</Option>);
                }
                if (res.data.email) {
                    os.push(<Option value="Email" key="Email">Email</Option>);
                }
                this.setState({ user: res.data, options: os });
            } else {
                message.error(res.message);
            }
        });
    }

    handleSendEmail = () => {
        if (!this.state.user || !this.state.user.userName) {
            message.error('请输入用户名/邮箱/手机号，点击找回密码');
            return;
        }
        this.setState({ submitting: true });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'user/forgotPasswordSendEmail',
                payload: {
                    resolve,
                    params: { userName: this.state.user.userName }
                },
            });
        }).then(res => {
            this.setState({ submitting: false });
            if (res.success === true) {
                this.setState({ disabledEmail: true });
                message.info("发送重置密码邮件成功,请登录邮箱查看");
            } else {
                message.warning(res.message);
            }
        });
    }

    onGetCaptcha = () => {
        if (!this.state.user || !this.state.user.userName) {
            message.error('请输入用户名/邮箱/手机号，点击找回密码');
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

        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'user/forgotPasswordSendPhone',
                payload: {
                    resolve,
                    params: { userName: this.state.user.userName }
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

    handleSubmit = e => {
        e.preventDefault();
        const { form, dispatch } = this.props;
        form.validateFields({ force: true }, (err, values) => {
            if (err) {
                return;
            }
            this.setState({ submitting: true });
            new Promise(resolve => {
                dispatch({
                    type: 'user/resetPasswordByPhone',
                    payload: {
                        resolve,
                        params: {
                            ...values,
                            userName: this.state.user.userName,
                            confirmPassword: values['confirm'],
                        }
                    },
                });
            }).then(res => {
                this.setState({ submitting: false });
                if (res.success === true) {
                    message.info("密码修改成功,1s后自动跳转登录页面");
                    setTimeout(() => {
                        router.push('/user/login');
                    }, 1000);
                } else {
                    message.warning(res.message);
                }
            });
        });
    };

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
        const { form } = this.props;
        const { getFieldDecorator } = form;
        const { count, help, visible } = this.state;
        return (
            <div className={styles.main}>
                <h3>忘记密码</h3>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem>
                        <Input
                            onChange={(e) => {
                                this.setState({ name: e.target.value });
                            }}
                            size="large" placeholder="用户名/邮箱/手机号" />
                    </FormItem>
                    {
                        this.state.options && this.state.options.length > 0 ?
                            <FormItem>
                                <InputGroup compact>
                                    <Select
                                        size="large"
                                        placeholder="验证方式"
                                        allowClear
                                        onChange={(e) => {
                                            let val = '';
                                            let status = this.state.status;
                                            if (e) {
                                                if (e == 'Phone') {
                                                    val = this.state.user.phone || '';
                                                    status = 1;
                                                }
                                                else if (e == 'Email') {
                                                    val = this.state.user.email || '';
                                                    status = 2;
                                                }
                                            } else {
                                                status = 0;
                                            }
                                            this.setState({ optionValue: val, status: status });
                                        }}
                                        style={{ width: '30%' }}>
                                        {this.state.options}
                                    </Select>
                                    <Input
                                        disabled
                                        value={this.state.optionValue}
                                        size="large"
                                        style={{ width: '70%' }}
                                    />
                                </InputGroup>
                            </FormItem> : null
                    }
                    {
                        this.state.status == 1 ?
                            <div>
                                <FormItem>
                                    <Row gutter={8}>
                                        <Col span={16}>
                                            {getFieldDecorator('code', {
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
                                                onClick={this.onGetCaptcha}
                                            >
                                                {count
                                                    ? `${count} s`
                                                    : formatMessage({ id: 'app.register.get-verification-code' })}
                                            </Button>
                                        </Col>
                                    </Row>
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
                            </div> : null
                    }
                    <FormItem>
                        {
                            this.state.status == 0 ?
                                <Button
                                    size="large"
                                    loading={this.state.submitting}
                                    className={styles.submit}
                                    type="primary"
                                    // htmlType="submit"
                                    onClick={this.handleGetUser}>
                                    找回密码</Button>
                                : (this.state.status == 1 ?
                                    <Button
                                        size="large"
                                        loading={this.state.submitting}
                                        className={styles.submit}
                                        type="primary"
                                        htmlType="submit">
                                        确认</Button> :
                                    <Button
                                        size="large"
                                        loading={this.state.submitting}
                                        className={styles.submit}
                                        type="primary"
                                        // htmlType="submit"
                                        disabled={this.state.disabledEmail}
                                        onClick={this.handleSendEmail}>
                                        发送重置密码邮件</Button>)
                        }

                        <Link className={styles.login} to="/user/login">
                            <FormattedMessage id="app.register.sing-in" />
                        </Link>
                    </FormItem>
                </Form>
            </div >
        );
    }
}

export default ForgotPassword;
