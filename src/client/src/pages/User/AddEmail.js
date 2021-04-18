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

@connect()
@Form.create()
class ResetPassword extends Component {
    constructor(props) {
        super(props);
        this.state = {
            userId: props.location.query.id,
            code: props.location.query.code,
            email: props.location.query.email,
            submitting: false,
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
                    type: 'user/addEmail',
                    payload: {
                        resolve,
                        params: {
                            ...values,
                            code: this.state.code,
                            userId: this.state.userId
                        }
                    },
                });
            }).then(res => {
                this.setState({ submitting: false });
                if (res.success === true) {
                    message.info("邮箱绑定成功,1s后自动跳转用户设置页面");
                    setTimeout(() => {
                        router.push('/account/settings/security');
                    }, 1000);
                } else {
                    message.error(res.message);
                }
            });
        });
    };

    render() {
        const { form } = this.props;
        const { getFieldDecorator } = form;
        return (
            <div className={styles.main}>
                <h3>绑定邮箱：{this.state.email}</h3>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem>
                        {getFieldDecorator('email', {
                            rules: [
                                {
                                    required: true,
                                    message: formatMessage({ id: 'validation.email.required' }),
                                },
                                {
                                    type: 'email',
                                    message: formatMessage({ id: 'validation.email.wrong-format' }),
                                },
                            ],
                        })(
                            <Input size="large" placeholder='请输入完整的邮箱地址' />
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
