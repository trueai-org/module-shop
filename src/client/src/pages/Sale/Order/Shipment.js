import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
  Form,
  Input,
  Button,
  Card,
  InputNumber,
  Icon,
  Checkbox,
  notification,
  Select,
  Spin,
  Table,
  Tabs,
  Cascader,
  Radio,
  Avatar,
  DatePicker,
  Tag,
  Row,
  Col,
} from 'antd';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
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
class OrderShipment extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      loading: false,
      submitting: false,
      no: props.location.query.no,
      current: {},

      products: [],
      productsLoading: false,

      orderStatusStr: '',
    };
  }

  columnsProduct = [
    {
      title: '图片',
      dataIndex: 'mediaUrl',
      align: 'center',
      width: 80,
      render: (text, record) => (
        <Fragment>
          <Avatar shape="square" size={48} src={record.mediaUrl} />
        </Fragment>
      ),
    },
    {
      title: '发货数量',
      dataIndex: 'quantityToShip',
      width: 120,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            precision={0}
            max={record.quantity - record.shippedQuantity}
            onChange={e => {
              record.quantityToShip = e;
              // let index = this.state.products.indexOf(record);
              // if (index >= 0) {
              //     let list = this.state.products.slice();
              //     list.splice(index, 1);
              //     record.productPrice = e;
              //     list.splice(index, 0, record);
              //     this.setState({ products: list });
              // }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '已发',
      dataIndex: 'shippedQuantity',
      width: 80,
    },
    {
      title: '库存',
      dataIndex: 'availableQuantity',
      render: val => (val == null ? '-' : val),
      width: 80,
    },
    {
      title: '下单',
      dataIndex: 'quantity',
      width: 80,
    },
    {
      title: '商品名称',
      dataIndex: 'name',
    },
  ];

  componentDidMount() {
    if (this.state.no) this.handleInit(this.state.no);
  }

  handleInit = no => {
    const { dispatch } = this.props;
    this.setState({ loading: true, current: {}, products: [] });
    new Promise(resolve => {
      dispatch({
        type: 'order/getByNo',
        payload: {
          resolve,
          params: { no },
        },
      });
    }).then(res => {
      this.setState({ loading: false });
      if (res.success === true) {
        this.setState({
          current: res.data,
          products: res.data.items,
        });
        let first = OrderStatus.find(c => c.key == res.data.orderStatus);
        if (first) {
          this.setState({ orderStatusStr: '[' + first.value + ']' });
        }
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleSubmit = e => {
    const { dispatch, form } = this.props;
    e.preventDefault();
    form.validateFieldsAndScroll((err, values) => {
      if (!err) {
        if (!this.state.products || this.state.products.length <= 0) {
          notification.warning({ message: '发货商品异常' });
          return;
        }
        if (!this.state.products.some(c => c.quantityToShip > 0)) {
          notification.warning({ message: '请输入发货数量' });
          return;
        }
        if (this.state.submitting) return;
        this.setState({ submitting: true });
        var params = {
          id: this.state.current.id,
          ...values,
        };
        params.items = this.state.products.filter(c => c.quantityToShip > 0).map(c => {
          return {
            quantityToShip: c.quantityToShip,
            orderItemId: c.orderItemId,
          };
        });

        // console.log(params);
        // return;
        new Promise(resolve => {
          dispatch({
            type: 'order/shipment',
            payload: {
              resolve,
              params,
            },
          });
        }).then(res => {
          this.setState({ submitting: false });
          if (res.success === true) {
            // router.push('./list');
            notification.success({ message: '发货成功,1s后自动跳转...' });
            setTimeout(() => {
              router.push({
                pathname: './shipment',
                query: {
                  no: this.state.no,
                },
              });
              router.go(0);
            }, 1000);
          } else {
            notification.error({ message: res.message });
          }
        });
      }
    });
  };

  handleGoBack = () => {
    router.goBack();
  };
  render() {
    const {
      form: { getFieldDecorator },
    } = this.props;
    const action = (
      <Fragment>
        {/* <Button onClick={this.handleGoBack}>
                    <Icon type="rollback" />返回上一级
                </Button> */}
        <Link to="./list">
          <Button>
            <Icon type="rollback" />
            {/* 返回订单列表 */}
          </Button>
        </Link>
      </Fragment>
    );
    return (
      <PageHeaderWrapper
        title="发货"
        content="当订单处于：已付款、待发货、发货中时，才允许执行发货操作，发货成功后自动刷新当前页面。"
        action={action}
      >
        <Card bordered={false}>
          <Form onSubmit={this.handleSubmit}>
            <FormItem>
              {getFieldDecorator('no', {
                initialValue: this.state.current.no,
                rules: [{ required: true, message: '请输入订单编号' }],
              })(
                <Input
                  onChange={e => {
                    if (e.target.value && e.target.value.length >= 18) {
                      this.setState({ no: e.target.value }, () => {
                        this.handleInit(this.state.no);
                      });
                    }
                  }}
                  size="large"
                  placeholder="请输入订单编号"
                />
              )}
            </FormItem>
            <Row gutter={16}>
              <Col span={8}>
                <Card type="inner" title="配送">
                  <FormItem>
                    {getFieldDecorator('trackingNumber', {
                      initialValue: '',
                      rules: [{ required: true, message: '请输入配送单号' }],
                    })(<Input placeholder="配送单号" />)}
                  </FormItem>
                  <FormItem>
                    {getFieldDecorator('totalWeight', {
                      initialValue: 0,
                    })(
                      <InputNumber
                        min={0}
                        precision={3}
                        style={{ width: 120 }}
                        placeholder="重量/kg"
                      />
                    )}
                    <span> kg</span>
                  </FormItem>
                  <FormItem>
                    {getFieldDecorator('adminComment', { initialValue: '' })(
                      <Input.TextArea rows={2} placeholder="管理员备注" />
                    )}
                  </FormItem>
                  <Button
                    style={{ width: '100%' }}
                    onClick={this.handleSubmit}
                    type="primary"
                    // icon="thunderbolt"
                    htmlType="submit"
                    loading={this.state.submitting}
                  >
                    发货
                  </Button>
                </Card>
              </Col>
              <Col span={16}>
                <Spin spinning={this.state.loading}>
                  <Card type="inner" title={`订单 ${this.state.orderStatusStr}`}>
                    {this.state.current.shippingAddress ? (
                      <div>
                        <div>
                          联系人：
                          <span style={{}}>{this.state.current.shippingAddress.contactName}</span>
                        </div>
                        <div>
                          联系电话：
                          <span style={{}}>{this.state.current.shippingAddress.phone}</span>
                        </div>
                        <div>
                          收货地址：
                          <span style={{}}>{this.state.current.shippingAddress.contactName}</span>
                        </div>
                      </div>
                    ) : (
                      <div>配送地址：-</div>
                    )}
                    <div style={{ marginBottom: 12 }}>
                      订单总额：
                      <span style={{ fontWeight: 'bold' }}>{this.state.current.orderTotal}</span> ¥
                    </div>
                    <Table
                      bordered={false}
                      rowKey={(record, index) => `order_item_${record.id}_i_${index}`} //{record => record.id}
                      pagination={false}
                      loading={this.state.productsLoading}
                      dataSource={this.state.products}
                      columns={this.columnsProduct}
                      scroll={{ x: 660 }}
                    />
                  </Card>
                </Spin>
              </Col>
            </Row>
          </Form>
        </Card>
      </PageHeaderWrapper>
    );
  }
}

export default OrderShipment;
