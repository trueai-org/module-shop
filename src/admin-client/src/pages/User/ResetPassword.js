import React, { Component } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import Link from 'umi/link';
import router from 'umi/router';
import styles from './Register.less';

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
class ResetPassword extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userName: props.location.query.userName,
            code: props.location.query.code,
            email: props.location.query.email,
            submitting: false,

            confirmDirty: false,
            visible: false,
            help: '',
        };
    }

    componentDidMount() {

    }

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
                    type: 'user/resetPasswordByEmail',
                    payload: {
                        resolve,
                        params: {
                            // ...values,
                            code: this.state.code,
                            userName: this.state.userName,
                            confirmPassword: values['confirm'],
                            password: values['password']
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
        const { help, visible } = this.state;
        return (
            <div className={styles.main}>
                <h3>重置密码</h3>
                <h3>邮箱：{this.state.email}</h3>
                <Form onSubmit={this.handleSubmit}>
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
                    <FormItem>
                        <Button
                            size="large"
                            loading={this.state.submitting}
                            className={styles.submit}
                            type="primary"
                            htmlType="submit">
                            确认</Button>
                        <Link className={styles.login} to="/user/login">
                            <FormattedMessage id="app.register.sing-in" />
                        </Link>
                    </FormItem>
                </Form>
            </div>
        );
    }
}

export default ResetPassword;
