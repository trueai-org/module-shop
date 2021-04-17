import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    Form, Input, Select, Button, Card, InputNumber, Icon,
    Checkbox, Upload, Modal, notification, Spin, DatePicker,
    Tooltip
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const { TextArea } = Input;
const Option = Select.Option;

const WidgetZoneWithId = [
    { key: 1, value: "Home Featured", desc: "首页头部" },
    { key: 2, value: "Home Main Content", desc: "首页主体" },
    { key: 3, value: "Home After Main Content", desc: "首页主体之后" }
];
const OrderBy = ['Newest', 'BestSelling', 'Discount'];

@connect()
@Form.create()
class WidgetProduct extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            id: props.location.query.id,
            loading: false,
            data: {
                setting: {}
            },
            categories: [],
            categoriesLoading: false,
            submitting: false,
        };
    }

    componentDidMount() {
        this.handleInit();
    }

    handleInitGet = () => {
        const { dispatch } = this.props;
        this.setState({ loading: true });
        new Promise(resolve => {
            dispatch({
                type: 'widget/getWidgetProduct',
                payload: {
                    resolve,
                    params: { id: this.state.id }
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({ data: res.data });
            } else {
                notification.error({ message: res.message });
            }
        });
    }

    handleInit = () => {
        if (this.state.id)
            this.handleInitGet();
        const { dispatch } = this.props;
        this.setState({ categoriesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'catalog/categories',
                payload: {
                    resolve,
                    params: {}
                },
            });
        }).then(res => {
            this.setState({ categoriesLoading: false });
            if (res.success === true) {
                this.setState({
                    categories: res.data
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    }


    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({ submitting: true });
                var params = {
                    id: this.state.id,
                    ...values
                };
                let type = 'widget/addWidgetProduct';
                if (this.state.id)
                    type = 'widget/editWidgetProduct';
                new Promise(resolve => {
                    dispatch({
                        type: type,
                        payload: {
                            resolve,
                            params
                        },
                    });
                }).then(res => {
                    this.setState({ submitting: false });
                    if (res.success === true) {
                        router.push('./list');
                    } else {
                        notification.error({ message: res.message, });
                    }
                });
            }
        });
    };

    render() {
        const { form: { getFieldDecorator, getFieldValue }, } = this.props;
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 4 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 12 },
                md: { span: 20 },
            },
        };
        const action = (
            <Fragment>
                <Button
                    onClick={this.handleSubmit}
                    icon="save" type="primary" htmlType="submit"
                    loading={this.state.submitting}>保存</Button>
                <Link to="./list">
                    <Button>
                        <Icon type="rollback" />
                    </Button>
                </Link>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="商品部件" action={action}>
                <Spin spinning={this.state.loading}>
                    <Card bordered={false}>
                        <Form onSubmit={this.handleSubmit}>
                            <FormItem
                                {...formItemLayout}
                                label={<span>部件名称</span>}>
                                {getFieldDecorator('name', {
                                    initialValue: this.state.data.name,
                                    rules: [{ required: true, message: '请输入部件名称' }],
                                })(<Input placeholder="部件名称" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>部件区域</span>}>
                                {getFieldDecorator('widgetZoneId', {
                                    rules: [{ required: true, message: '请选择部件区域' }],
                                    initialValue: this.state.data.widgetZoneId || '', valuePropName: 'value'
                                })(<Select allowClear={true}>
                                    {WidgetZoneWithId.map(c => <Option value={c.key} key={c.key}>{c.desc}</Option>)}
                                </Select>)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>发布开始时间</span>}>
                                {getFieldDecorator('publishStart', {
                                    initialValue: this.state.data.publishStart
                                        ? moment(this.state.data.publishStart, 'YYYY-MM-DD HH:mm:ss')
                                        : null
                                })(<DatePicker showTime
                                    format='YYYY-MM-DD HH:mm:ss'
                                    placeholder="发布开始时间" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>发布结束时间</span>}>
                                {getFieldDecorator('publishEnd', {
                                    initialValue: this.state.data.publishEnd
                                        ? moment(this.state.data.publishEnd, 'YYYY-MM-DD HH:mm:ss')
                                        : null
                                })(<DatePicker showTime
                                    format='YYYY-MM-DD HH:mm:ss'
                                    placeholder="发布结束时间" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>显示顺序</span>}>
                                {
                                    getFieldDecorator('displayOrder', { initialValue: this.state.data.displayOrder })(
                                        <InputNumber placeholder="显示顺序" />
                                    )
                                }
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>商品分类<Tooltip title="如果未选择分类，则表示全部分类；选择父级分类包括子级分类">
                                    <Icon type="info-circle" theme="filled" />
                                </Tooltip></span>}>
                                {getFieldDecorator('setting.categoryId', {
                                    // rules: [{ required: true, message: '请选择商品分类' }],
                                    initialValue: this.state.data.setting.categoryId || '',
                                    valuePropName: 'value'
                                })(<Select loading={this.state.categoriesLoading} allowClear={true}>
                                    <Option value='' key=''>全部</Option>
                                    {this.state.categories.map(c => <Option value={c.id} key={c.id}>{c.name}</Option>)}
                                </Select>)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>商品数量</span>}>
                                {
                                    getFieldDecorator('setting.itemCount', { initialValue: this.state.data.setting.itemCount })(
                                        <InputNumber min={0} precision={0} placeholder="商品数量" />
                                    )
                                }
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>排序方式</span>}>
                                {getFieldDecorator('setting.orderBy', {
                                    rules: [{ required: true, message: '请选择排序方式' }],
                                    initialValue: this.state.data.setting.orderBy || OrderBy[0],
                                    valuePropName: 'value'
                                })(<Select loading={this.state.categoriesLoading}>
                                    {OrderBy.map(c => <Option value={c} key={c}>{c}</Option>)}
                                </Select>)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>只显示精品</span>}>
                                {
                                    getFieldDecorator('setting.featuredOnly', { initialValue: this.state.data.setting.featuredOnly || false, valuePropName: 'checked' })(
                                        <Checkbox />
                                    )
                                }
                            </FormItem>
                        </Form>
                    </Card>
                </Spin>
            </PageHeaderWrapper>
        );
    }
}

export default WidgetProduct;