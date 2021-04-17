import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select,
    Tag, Icon, Redio, Menu, Dropdown, Switch, Row, Col, InputNumber, DatePicker, Checkbox, Spin,
    Tooltip, Badge, Avatar
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';

import router from 'umi/router';
import Link from 'umi/link';

const { RangePicker } = DatePicker;
const FormItem = Form.Item;
const Option = Select.Option;
const ShippingStatus = [
    { key: 0, value: '无需配送', color: '' },
    { key: 20, value: '未发货', color: '#f50' },
    { key: 25, value: '部分发货', color: '#2db7f5' },
    { key: 30, value: '已发货', color: '#108ee9' },
    { key: 40, value: '已收货', color: '#87d068' },
];

const OrderStatus = [
    { key: 0, value: '新订单', color: 'purple' },
    { key: 10, value: '挂起', color: 'red' },
    { key: 20, value: '待付款', color: 'orange' },
    { key: 25, value: '付款失败', color: 'red' },
    { key: 30, value: '已付款', color: 'lime' },
    { key: 40, value: '发货中', color: 'cyan' },
    { key: 50, value: '已发货', color: 'blue' },
    { key: 60, value: '交易成功', color: 'green' },
    { key: 70, value: '交易取消', color: '' },
];

@connect()
@Form.create()
class ShipmentList extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,

            pageNum: 1,
            pageSize: 10,
            predicate: '',
            reverse: true,
            pageData: {
                list: [],
                pagination: {},
            },

            queryParam: {},
        };
    }

    columns = [
        // {
        //     title: 'ID',
        //     dataIndex: 'id',
        //     // fixed: 'left',
        //     sorter: true,
        //     defaultSortOrder: 'descend',
        //     width: 100,
        // },
        {
            title: '配送单号',
            dataIndex: 'trackingNumber',
            sorter: true,
            width: 180,
        },
        {
            title: '订单编号',
            dataIndex: 'orderNo',
            width: 180,
        },
        {
            title: '订单状态',
            dataIndex: 'orderStatus',
            width: 120,
            render: val => {
                if (val || val === 0) {
                    let first = OrderStatus.find(c => c.key == val);
                    if (first) {
                        return <Tag color={first.color}>{first.value}</Tag>;
                    }
                }
                return <Tag>无</Tag>;
            },
        },
        {
            title: '配送状态',
            dataIndex: 'shippingStatus',
            width: 120,
            render: val => {
                if (val || val === 0) {
                    let first = ShippingStatus.find(c => c.key == val);
                    if (first) {
                        return <Tag color={first.color}>{first.value}</Tag>;
                    }
                }
                return '-';
            },
        },
        {
            title: '总重量/kg',
            dataIndex: 'totalWeight',
            sorter: true,
        },
        {
            title: '操作人',
            dataIndex: 'createdBy',
        },
        {
            title: '发货时间',
            dataIndex: 'shippedOn',
            sorter: true,
            width: 120,
            render: val => val ? <span>{moment(val).format('YYYY-MM-DD HH:mm:ss')}</span> : null,
        },
        {
            title: '收货时间',
            dataIndex: 'deliveredOn',
            sorter: true,
            width: 120,
            render: val => val ? <span>{moment(val).format('YYYY-MM-DD HH:mm:ss')}</span> : null,
        }
    ];

    componentDidMount() {
        this.queryData(this.state.queryParam);
    }

    queryDataFirst = () => {
        this.setState(
            {
                pageNum: 1,
            },
            () => {
                this.queryData();
            }
        );
    };

    queryData = () => {
        this.setState({ loading: true });
        const { dispatch, form } = this.props;

        let search = this.state.queryParam;
        form.validateFields((err, fieldsValue) => {
            if (err) return;
            search = {
                ...fieldsValue,
            };

            if (search.shippedOn && search.shippedOn.length == 2) {
                search.shippedOnStart = search.shippedOn[0].format('YYYY-MM-DD HH:mm:ss');
                search.shippedOnEnd = search.shippedOn[1].format('YYYY-MM-DD HH:mm:ss');
                search.shippedOn = {};
            }
            this.setState({ queryParam: search });
        });

        let params = {
            search: search,
            pagination: {
                current: this.state.pageNum,
                pageSize: this.state.pageSize,
            },
            sort: {
                predicate: this.state.predicate,
                reverse: this.state.reverse,
            },
        };

        new Promise(resolve => {
            dispatch({
                type: 'shipment/grid',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({
                    pageData: res.data,
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    handleSearch = e => {
        const { form } = this.props;
        e.preventDefault();
        form.validateFields((err, fieldsValue) => {
            if (err) return;
            let search = {
                ...fieldsValue,
            };

            this.setState({ queryParam: search }, () => {
                this.queryData();
            });
        });
    };

    handleStandardTableChange = (pagination, filtersArg, sorter) => {
        this.setState({
            pageNum: pagination.current,
            pageSize: pagination.pageSize,
            predicate: sorter.field ? sorter.field : this.state.predicate,
            reverse: sorter.order == 'descend',
        }, () => {
            this.queryData();
        });
    };

    renderForm() {
        const {
            form: { getFieldDecorator },
        } = this.props;
        return (
            <Form onSubmit={this.handleSearch} layout="inline">
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col span={12}>
                        <FormItem label="配送单号">
                            {getFieldDecorator('trackingNumber')(
                                <Input style={{}} allowClear placeholder="配送单号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem label="订单编号">
                            {getFieldDecorator('orderNo')(
                                <Input style={{}} allowClear placeholder="订单编号" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col>
                        <FormItem label="发货时间">{getFieldDecorator('shippedOn')(<RangePicker showTime format="YYYY-MM-DD HH:mm:ss" />)}</FormItem>
                    </Col>
                </Row>
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col span={12}>
                        <span>
                            <Button type="primary" htmlType="submit" icon="search">查询</Button>
                            <Button style={{ marginLeft: 8 }} onClick={this.handleFormReset} icon="undo">重置</Button>
                        </span>
                    </Col>
                </Row>
            </Form >
        );
    }

    handleFormReset = () => {
        const { form, dispatch } = this.props;
        form.resetFields();
        this.setState({
            formValues: {},
        });
    };

    expandedRowRender = (record) => {
        const columns = [
            {
                title: '图片',
                dataIndex: 'productMediaUrl',
                align: 'center',
                width: 64,
                render: (text) => <Avatar shape="square" size={32} src={text} />
            },
            {
                title: '发货数量',
                dataIndex: 'quantity',
                width: 120
            },
            {
                title: <Tooltip title="已发货数量/下单数量">已发/下单</Tooltip>,
                dataIndex: 'shippedQuantity',
                width: 160,
                render: (text, itemRecord) => (text + '/' + itemRecord.orderedQuantity)
            },
            {
                title: '商品名称',
                dataIndex: 'productName'
            },
        ];
        return (
            <Table
                columns={columns}
                dataSource={record.items}
                pagination={false}
                bordered={false}
                rowKey={(record, index) => `shipment_item_${record.id}_i_${index}`}
            />
        );
    };

    render() {
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
            },
        };

        return (
            <PageHeaderWrapper title="发货记录"  >
                <div>
                    <Card bordered={false}>
                        <div>{this.renderForm()}</div>
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered={false}
                            onChange={this.handleStandardTableChange}
                            scroll={{ x: 1260 }}
                            expandedRowRender={this.expandedRowRender}
                        />
                    </Card>
                </div>
            </PageHeaderWrapper>
        );
    }
}

export default ShipmentList;
