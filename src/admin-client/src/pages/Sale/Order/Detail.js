import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    Form, Input, Button, Card, InputNumber, Icon, Checkbox, notification, Select, Spin,
    Table, Tabs, Cascader, Radio, Avatar, DatePicker, Tag, Steps, Menu, Dropdown, Row, Col,
    Badge, Tooltip, Divider, Popover, Switch
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

import ProductCommponent from './ProductCommponent';

const FormItem = Form.Item;
const { Option } = Select;
const TabPane = Tabs.TabPane;
const { TextArea } = Input;
const RadioGroup = Radio.Group;
const ShippingMethod = [{ key: 0, value: '免费' }, { key: 1, value: '标准' }];
const PaymentType = [{ key: 0, value: '在线支付' }, { key: 1, value: '货到付款' }];
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
const PaymentMethod = [
    { key: 0, value: '支付宝', color: '' },
    { key: 1, value: '微信', color: '' },
];
const RefundStatus = [
    { key: 0, value: '等待退款', color: '' },
    { key: 10, value: '退款成功', color: '' },
    { key: 20, value: '退款取消', color: '' },
    { key: 30, value: '关闭', color: '' },
    { key: 40, value: '退款冻结', color: '' },
];


const CancelOrderStatus = [0, 20, 25];
const DeleteOrderStatus = [60, 70];
const OnHoldNotOrderStatus = [10, 60, 70];
const NotMoreStatus = [10];
const DeliveryOrderStatus = [30, 40];
const DeliveryShippingStatus = [null, 0, 20, 25];

import classNames from 'classnames';
import DescriptionList from '@/components/DescriptionList';
import Debounce from 'lodash-decorators/debounce';
import Bind from 'lodash-decorators/bind';

const getWindowWidth = () => window.innerWidth || document.documentElement.clientWidth;

const { Step } = Steps;
const { Description } = DescriptionList;
const ButtonGroup = Button.Group;

@connect()
@Form.create()
class OrderDetail extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            id: props.location.query.id,
            current: {},
            currentStep: 0,

            products: [],
            productsLoading: false,

            histories: [],
            historiesLoading: false,

            stepDirection: 'horizontal',
        };
    }

    columnsHistories = [
        {
            title: '操作时间',
            dataIndex: 'createdOn',
            width: 160,
            render: val => <span>{moment(val).format('YYYY-MM-DD HH:mm:ss')}</span>,
        },
        {
            title: '状态',
            dataIndex: 'newStatus',
            width: 200,
            render: (text, record) => {
                let strs = [];
                if (record.oldStatus || record.oldStatus === 0) {
                    let first = OrderStatus.find(c => c.key == record.oldStatus);
                    if (first) {
                        strs.push(
                            <Tag key={`old_${record.id}`} color={first.color}>
                                {first.value}
                            </Tag>
                        );
                    }
                    strs.push(
                        <span key={`c_${record.id}`} style={{ marginRight: 8 }}>→</span>
                    );
                }
                if (record.newStatus || record.newStatus === 0) {
                    let first = OrderStatus.find(c => c.key == record.newStatus);
                    if (first) {
                        strs.push(
                            <Tag key={`new_${record.id}`} color={first.color}>
                                {first.value}
                            </Tag>
                        );
                    }
                }
                return strs;
            },
        },
        {
            title: '操作人',
            dataIndex: 'createdByFullName',
        },
        {
            title: '备注',
            dataIndex: 'note',
        },
    ];

    columnsProduct = [
        {
            title: '商品单价',
            dataIndex: 'productPrice',
            width: 120,
        },
        {
            title: '折扣总额',
            dataIndex: 'discountAmount',
            width: 120,
        },
        {
            title: '下单数量',
            dataIndex: 'quantity',
            width: 120,
        },
        {
            title: '已发数量',
            dataIndex: 'shippedQuantity',
            width: 120,
        },
        {
            title: '图片',
            dataIndex: 'mediaUrl',
            align: 'center',
            width: 64,
            render: (text, record) => (
                <Fragment>
                    <Avatar shape="square" size={32} src={record.mediaUrl} />
                </Fragment>
            ),
        },
        {
            title: '商品名称',
            dataIndex: 'name',
        },
    ];

    componentDidMount() {
        if (this.state.id) {
            this.handleInit(this.state.id);
            this.handleLoadHistory(this.state.id);
        }

        this.setStepDirection();
        window.addEventListener('resize', this.setStepDirection, { passive: true });
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.setStepDirection);
        this.setStepDirection.cancel();
    }

    handleLoadHistory = orderId => {
        const { dispatch } = this.props;
        this.setState({ historiesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'order/histories',
                payload: {
                    resolve,
                    params: { orderId },
                },
            });
        }).then(res => {
            this.setState({ historiesLoading: false });
            if (res.success === true) {
                this.setState({
                    histories: res.data,
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    };

    handleInit = id => {
        const { dispatch } = this.props;
        this.setState({ loading: true });
        new Promise(resolve => {
            dispatch({
                type: 'order/get',
                payload: {
                    resolve,
                    params: { id },
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                let step = this.state.currentStep;
                switch (res.data.orderStatus) {
                    case 60:
                        step = 3;
                        break;
                    case 50:
                        step = 2;
                        break;
                    case 30:
                    case 40:
                        step = 1;
                        break;
                    case 0:
                    case 20:
                    case 25:
                        step = 0;
                        break;
                    default:
                        step = 0; // 交易取消 挂起
                        break;
                }
                this.setState({
                    current: res.data,
                    products: res.data.items,
                    currentStep: step
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    };

    @Bind()
    @Debounce(200)
    setStepDirection() {
        const { stepDirection } = this.state;
        const w = getWindowWidth();
        if (stepDirection !== 'vertical' && w <= 576) {
            this.setState({
                stepDirection: 'vertical',
            });
        } else if (stepDirection !== 'horizontal' && w > 576) {
            this.setState({
                stepDirection: 'horizontal',
            });
        }
    }

    render() {
        const { stepDirection, loading, current } = this.state;
        const popoverContent = (
            <div style={{ width: 160 }}>
                <div style={{ marginTop: 4 }}>耗时：{

                }</div>
            </div>
        );
        const customDot = (dot, { status }) =>
            status === 'process' ? (
                <Popover placement="topLeft" arrowPointAtCenter content={popoverContent}>
                    {dot}
                </Popover>
            ) : (dot);

        const action = (
            <Fragment>
                <ButtonGroup>
                    <Button icon=""
                        onClick={() => {
                            router.push({
                                pathname: './edit',
                                query: {
                                    id: this.state.id,
                                },
                            });
                        }}
                        type="primary">
                        <Icon type="edit" />
                    </Button>
                    <Button icon=""
                        onClick={() => {
                            router.push('./list')
                        }}>
                        <Icon type="rollback" />
                    </Button>
                </ButtonGroup>
            </Fragment>
        );
        const description = (
            <DescriptionList size="small" col="2">
                <Description term="客户">{current.customerName}</Description>
                <Description term="联系人">{current.contactName}</Description>
                <Description term="下单时间">{current.createdOn}</Description>
                <Description term="支付时间">{current.paymentOn}</Description>
                <Description term="发货时间">{current.shippedOn}</Description>
                <Description term="收货时间">{current.deliveredOn}</Description>
                <Description term="管理员备注">{current.adminNote}</Description>
            </DescriptionList>
        );
        const extra = (
            <Row>
                <Col xs={24} sm={12}>
                    <div>状态</div>
                    <div>{OrderStatus.find(c => c.key == current.orderStatus)
                        ? OrderStatus.find(c => c.key == current.orderStatus).value : ''}
                    </div>
                </Col>
                <Col xs={24} sm={12}>
                    <div>订单金额</div>
                    <div>¥ {current.orderTotal}</div>
                </Col>
            </Row>
        );
        return (
            <PageHeaderWrapper
                title={`订单编号：${current.no}`}
                content={description}
                extraContent={extra}
                action={action}>
                <Spin spinning={this.state.loading}>
                    <Card title="订单进度" style={{ marginBottom: 24 }} bordered={false}>
                        <Steps direction={stepDirection} progressDot={customDot} current={this.state.currentStep}>
                            <Step title="下单"
                                description={(
                                    <div>
                                        <Fragment>{current.customerName}</Fragment>
                                        <div>{current.createdOn}</div>
                                    </div>
                                )}
                            />
                            <Step title="支付" description={(
                                <div>
                                    <Fragment>￥ {current.paymentFeeAmount}</Fragment>
                                    <div>{current.paymentOn}</div>
                                </div>
                            )}
                            />
                            <Step title="发货"
                                description={(
                                    <div>
                                        <Fragment>￥ {current.shippingFeeAmount}</Fragment>
                                        <div>{current.shippedOn}</div>
                                    </div>
                                )}
                            />
                            <Step title="完成"
                                description={(
                                    <div>
                                        <div>{current.deliveredOn}</div>
                                    </div>
                                )}
                            />
                        </Steps>
                    </Card>
                    <Card title="基本信息" style={{ marginBottom: 24 }} bordered={false}>
                        <DescriptionList style={{ marginBottom: 24 }}>
                            <Description term="订单状态">
                                {current.orderStatus ? OrderStatus.find(c => c.key == current.orderStatus).value : null}
                            </Description>
                            <Description term="配送状态">
                                {current.shippingStatus ? ShippingStatus.find(c => c.key == current.shippingStatus).value : null}
                            </Description>
                            <Description term="配送方式">
                                {current.shippingMethod ? shippingMethod.find(c => c.key == current.shippingMethod).value : null}
                            </Description>
                            <Description term="运费">
                                {current.shippingFeeAmount}
                            </Description>
                            <Description term="发货时间">
                                {current.shippedOn}
                            </Description>
                            <Description term="收货时间">
                                {current.deliveredOn}
                            </Description>
                            <Description term="付款类型">
                                {current.paymentType ? PaymentType.find(c => c.key == current.paymentType).value : null}
                            </Description>
                            <Description term="付款方式">
                                {current.paymentMethod ? PaymentMethod.find(c => c.key == current.paymentMethod).value : null}
                            </Description>
                            <Description term="付款金额">
                                {current.paymentFeeAmount}
                            </Description>
                            <Description term="付款时间">
                                {current.paymentOn}
                            </Description>
                            <Description term="订单折扣">
                                {current.discountAmount}
                            </Description>
                            <Description term="订单总额">
                                {current.orderTotal}
                            </Description>
                            <Description term="下单备注">
                                {current.orderNote}
                            </Description>
                            <Description term="交易取消时间">
                                {current.cancelOn}
                            </Description>
                            <Description term="交易取消原因">
                                {current.cancelReason}
                            </Description>
                        </DescriptionList>
                        <Card type="inner" title="退款信息" style={{ marginBottom: 24 }}>
                            <DescriptionList title="">
                                <Description term="退款状态">{current.refundStatus ?
                                    RefundStatus.find(c => c.key == current.refundStatus).value : null}</Description>
                                <Description term="退款金额">{current.refundAmount}</Description>
                                <Description>&nbsp;</Description>
                                <Description term="退款时间">{current.refundOn}</Description>
                                <Description term="退款原因">{current.refundReason}</Description>
                            </DescriptionList>
                        </Card>
                        <Card type="inner" title="配送 & 账单">
                            {
                                current.shippingAddress ? <DescriptionList size="small" title="配送地址">
                                    <Description term="联系人">{current.shippingAddress.contactName}</Description>
                                    <Description term="手机">{current.shippingAddress.phone}</Description>
                                    <Description term="邮箱">{current.shippingAddress.email}</Description>
                                    <Description term="国家">{current.shippingAddress.contactName}</Description>
                                    <Description term="省市区">{current.shippingAddress.contactName}</Description>
                                    <Description term="城市">{current.shippingAddress.city}</Description>
                                    <Description term="邮编">{current.shippingAddress.zipCode}</Description>
                                    <Description term="公司">{current.shippingAddress.company}</Description>
                                    <Description term="街道地址">{current.shippingAddress.addressLine1}</Description>
                                    <Description term="街道地址2">{current.shippingAddress.addressLine2}</Description>
                                </DescriptionList> : <DescriptionList size="small" title="无配送地址"></DescriptionList>
                            }
                            <Divider style={{ margin: '16px 0' }} />
                            {
                                current.billingAddress ? <DescriptionList size="small" title="账单地址">
                                    <Description term="联系人">{current.billingAddress.contactName}</Description>
                                    <Description term="手机">{current.billingAddress.phone}</Description>
                                    <Description term="邮箱">{current.billingAddress.email}</Description>
                                    <Description term="国家">{current.billingAddress.contactName}</Description>
                                    <Description term="省市区">{current.billingAddress.contactName}</Description>
                                    <Description term="城市">{current.billingAddress.city}</Description>
                                    <Description term="邮编">{current.billingAddress.zipCode}</Description>
                                    <Description term="公司">{current.billingAddress.company}</Description>
                                    <Description term="街道地址">{current.billingAddress.addressLine1}</Description>
                                    <Description term="街道地址2">{current.billingAddress.addressLine2}</Description>
                                </DescriptionList> : <DescriptionList size="small" title="无账单地址"></DescriptionList>
                            }
                        </Card>
                    </Card>
                    <Card bordered={false} title="商品信息" style={{ marginBottom: 24 }}>
                        <Table
                            bordered={false}
                            rowKey={(record, index) => `product_${record.id}_i_${index}`}
                            pagination={false}
                            loading={this.state.productsLoading}
                            dataSource={this.state.products}
                            columns={this.columnsProduct}
                            scroll={{ x: 800 }}
                        />
                    </Card>
                    <Card bordered={false} title="操作记录">
                        <Table
                            bordered={false}
                            rowKey={(record, index) => `order_history_${record.id}_i_${index}`}
                            pagination={false}
                            loading={this.state.historiesLoading}
                            dataSource={this.state.histories}
                            columns={this.columnsHistories}
                        />
                    </Card>
                </Spin>
            </PageHeaderWrapper>
        );
    }
}

export default OrderDetail;