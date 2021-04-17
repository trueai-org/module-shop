import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Form, Input, Button, Card, InputNumber, Icon, Checkbox, notification
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;

const action = (
    <Fragment>
        <Link to="./list">
            <Button>
                <Icon type="rollback" />
            </Button>
        </Link>
    </Fragment>
);

@connect()
@Form.create()
class CountryEdit extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            submitting: false,
            id: props.location.query.id,
            current: {}
        };
    }

    handleInit = () => {
        const { dispatch } = this.props;
        this.setState({ loading: true });
        if (this.state.id) {
            new Promise(resolve => {
                dispatch({
                    type: 'country/get',
                    payload: {
                        resolve,
                        params: { id: this.state.id }
                    },
                });
            }).then(res => {
                // this.setState({ loading: false });
                if (res.success === true) {
                    this.setState({ current: res.data });
                } else {
                    notification.error({ message: res.message });
                }
            });
        }
    }

    componentDidMount() {
        this.handleInit();
    }

    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                if (this.state.submitting)
                    return;
                this.setState({ submitting: true });
                var params = {
                    id: this.state.id,
                    ...values
                };
                new Promise(resolve => {
                    dispatch({
                        type: 'country/edit',
                        payload: {
                            resolve,
                            params
                        },
                    });
                }).then(res => {
                    this.setState({
                        submitting: false,
                    }, () => {
                        if (res.success === true) {
                            router.push('./list');
                        } else {
                            notification.error({ message: res.message, });
                        }
                    });
                });
            }
        });
    };

    render() {
        const {
            form: { getFieldDecorator, getFieldValue },
        } = this.props;
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 7 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 12 },
                md: { span: 10 },
            },
        };
        const submitFormLayout = {
            wrapperCol: {
                xs: { span: 24, offset: 0 },
                sm: { span: 10, offset: 7 },
            },
        };
        return (
            <PageHeaderWrapper title="国家 - 编辑" action={action}>
                <Card bordered={false}>
                    <Form onSubmit={this.handleSubmit}>
                        <FormItem
                            {...formItemLayout}
                            label={<span>名称</span>}>
                            {getFieldDecorator('name', {
                                initialValue: this.state.current.name,
                                rules: [{ required: true, message: '请输入国家名称' }],
                            })(<Input placeholder="国家名称" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>两个字母ISO代码</span>}>
                            {getFieldDecorator('twoLetterIsoCode', { initialValue: this.state.current.twoLetterIsoCode })(
                                <Input placeholder="两个字母ISO代码" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>三个字母ISO代码</span>}>
                            {getFieldDecorator('threeLetterIsoCode', { initialValue: this.state.current.threeLetterIsoCode })(
                                <Input placeholder="三个字母ISO代码" />
                            )}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>ISO数字代码</span>}>
                            {
                                getFieldDecorator('numericIsoCode', { initialValue: this.state.current.numericIsoCode })(
                                    <InputNumber style={{ width: '100%' }} placeholder="ISO数字代码" />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>允许账单</span>}>
                            {
                                getFieldDecorator('isBillingEnabled', { initialValue: this.state.current.isBillingEnabled || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>允许配送</span>}>
                            {
                                getFieldDecorator('isShippingEnabled', { initialValue: this.state.current.isShippingEnabled || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>启用市</span>}>
                            {
                                getFieldDecorator('isCityEnabled', { initialValue: this.state.current.isCityEnabled || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>启用区</span>}>
                            {
                                getFieldDecorator('isDistrictEnabled', { initialValue: this.state.current.isDistrictEnabled || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>已发布</span>}>
                            {
                                getFieldDecorator('isPublished', { initialValue: this.state.current.isPublished || false, valuePropName: 'checked' })(
                                    <Checkbox />
                                )
                            }
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>显示顺序</span>}>
                            {
                                getFieldDecorator('displayOrder', { initialValue: this.state.current.displayOrder })(
                                    <InputNumber style={{ width: '100%' }} placeholder="显示顺序" />
                                )
                            }
                        </FormItem>
                        <FormItem {...submitFormLayout}>
                            <Button type="primary" htmlType="submit" loading={this.state.submitting}>
                                保存
                            </Button>
                        </FormItem>
                    </Form>
                </Card>
            </PageHeaderWrapper>
        );
    }
}

export default CountryEdit;
