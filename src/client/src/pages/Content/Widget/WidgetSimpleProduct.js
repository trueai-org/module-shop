import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    Form, Input, Select, Button, Card, InputNumber, Icon,
    Checkbox, Upload, Modal, notification, Spin, DatePicker,
    Table, Radio, Avatar
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';
import ProductCommponent from '../../Sale/Order/ProductCommponent';
import { formatBool } from '@/utils/utils';

const FormItem = Form.Item;
const { TextArea } = Input;
const Option = Select.Option;

const WidgetZoneWithId = [
    { key: 1, value: "Home Featured", desc: "首页头部" },
    { key: 2, value: "Home Main Content", desc: "首页主体" },
    { key: 3, value: "Home After Main Content", desc: "首页主体之后" }
];

@connect()
@Form.create()
class WidgetSimpleProduct extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            id: props.location.query.id,
            loading: false,
            data: {
                setting: {}
            },
            submitting: false,

            // 添加商品
            products: [],
            productsLoading: false,
            visibleProduct: false,
            queryProductLoading: false,
            queryParam: {},
            search: {},
            pageNum: 1,
            pageSize: 5,
            predicate: 'id',
            reverse: true,
            pageData: {
                list: [],
                pagination: {}
            },
        };
    }

    columnsProduct = [
        {
            title: '商品名称',
            dataIndex: 'name'
        },
        {
            title: '是否发布',
            dataIndex: 'isPublished',
            width: 100,
            render: (val) => formatBool(val)
        },
        {
            title: '操作',
            key: 'operation',
            align: 'center',
            width: 64,
            fixed: 'right',
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button onClick={() => this.handleRemoveProduct(record)} icon="close" type="danger" size="small"></Button>
                    </Button.Group>
                </Fragment>
            )
        },
    ];


    componentDidMount() {
        this.handleInit();
    }

    handleInitGet = () => {
        const { dispatch } = this.props;
        this.setState({ loading: true });
        new Promise(resolve => {
            dispatch({
                type: 'widget/getWidgetSimpleProduct',
                payload: {
                    resolve,
                    params: { id: this.state.id }
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({
                    data: res.data,
                    products: res.data.setting.products
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    }

    handleInit = () => {
        if (this.state.id)
            this.handleInitGet();
    }

    handleSubmit = e => {
        const { dispatch, form } = this.props;
        e.preventDefault();
        form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({ submitting: true });
                var params = {
                    id: this.state.id,
                    setting: {
                        products: this.state.products
                    },
                    ...values
                };
                let type = 'widget/addWidgetSimpleProduct';
                if (this.state.id)
                    type = 'widget/editWidgetSimpleProduct';
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

    handleRemoveProduct = (record) => {
        this.setState(({ products }) => {
            let index = products.indexOf(record);
            let list = products.slice();
            list.splice(index, 1);
            return {
                products: list,
            };
        });
    }

    saveFormRef = (formRef) => {
        this.formRef = formRef;
    }

    showProductModal = () => {
        this.setState({ visibleProduct: true }, () => {
            this.handleSearch();
        });
    }

    handleProductCancel = () => {
        this.setState({ visibleProduct: false });
    }

    handleSearch = (name) => {
        const { dispatch } = this.props;
        this.setState({ queryProductLoading: true });
        let params = {
            search: {
                name,
                // isPublished: true
            },
            pagination: {
                current: this.state.pageNum,
                pageSize: this.state.pageSize
            },
            sort: {
                predicate: this.state.predicate,
                reverse: this.state.reverse
            }
        };
        new Promise(resolve => {
            dispatch({
                type: 'catalog/products',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ queryProductLoading: false });
            if (res.success === true) {
                this.setState({ pageData: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
    }

    handleStandardTableChange = (pagination, filtersArg, sorter) => {
        this.setState({
            pageNum: pagination.current,
            pageSize: pagination.pageSize,
        }, () => {
            this.handleSearch();
        });
    };

    onSelectChange = (selectedRowKeys) => {
        this.setState({
            visibleProduct: false,
            productsLoading: true
        });
        let ids = [];
        ids = selectedRowKeys
        if (!ids || ids.length <= 0)
            return;
        let pros = this.state.products;
        ids.forEach(id => {
            var old = pros.find(c => c.id == id);
            if (old)
                return;
            var first = this.state.pageData.list.find(c => c.id == id);
            if (first) {
                let pro = {
                    id: first.id,
                    name: first.name,
                    isPublished: first.isPublished
                };
                pros.push(pro);
            }
        });
        this.setState({
            products: pros,
            productsLoading: false
        });
    }

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
        const pagination = {
            showQuickJumper: true,
            showSizeChanger: true,
            pageSizeOptions: ['5', '10', '50', '100'],
            defaultPageSize: this.state.pageSize,
            defaultCurrent: this.state.pageNum,
            current: this.state.pageNum,
            pageSize: this.state.pageSize,
            total: this.state.pageData.pagination.total || 0,
            showTotal: (total, range) => {
                return `${range[0]}-${range[1]} 条 , 共 ${total} 条`;
            }
        };
        return (
            <PageHeaderWrapper title="简单商品部件" action={action}>
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
                                label={<span>商品</span>}>
                                <Button icon="plus" type="primary" style={{ marginBottom: 16 }} onClick={this.showProductModal}>添加商品</Button>
                                <Table bordered={false}
                                    rowKey={(record, index) => `product_${record.id}_i_${index}`} //{record => record.id}
                                    pagination={false}
                                    loading={this.state.productsLoading}
                                    dataSource={this.state.products}
                                    columns={this.columnsProduct}
                                // scroll={{ x: 960 }}
                                />
                            </FormItem>


                        </Form>
                    </Card>
                </Spin>
                <ProductCommponent
                    pagination={pagination}
                    pageData={this.state.pageData}
                    visible={this.state.visibleProduct}
                    loading={this.state.queryProductLoading}
                    wrappedComponentRef={this.saveFormRef}
                    handleSearch={this.handleSearch}
                    onCancel={this.handleProductCancel}
                    onChange={this.handleStandardTableChange}
                    onOk={(val) => { this.onSelectChange(val) }}
                />
            </PageHeaderWrapper>
        );
    }
}

export default WidgetSimpleProduct;