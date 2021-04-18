import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
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

@connect()
@Form.create()
class EditOrder extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      loading: false,
      submitting: false,
      id: props.location.query.id,
      current: {},

      users: [],
      usersLoading: false,

      products: [],
      productsLoading: false,

      countries: [],
      countriesLoading: false,

      provinces: [],
      provinces2: [],

      userAddresses: {
        addresses: [],
      },
      userAddressesLoading: false,

      defaultShippingAddressId: '', //客户默认配送地址
      defaultBillingAddressId: '',
      shippingMethod: 0,

      //组件
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
        pagination: {},
      },

      histories: [],
      historiesLoading: false,
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
            <span key={`c_${record.id}`} style={{ marginRight: 8 }}>
              →
            </span>
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
      title: '商品名称',
      dataIndex: 'name',
    },
    {
      title: '图片',
      dataIndex: 'mediaUrl',
      align: 'center',
      width: 64,
      // fixed: 'right',
      render: (text, record) => (
        <Fragment>
          <Avatar shape="square" size={32} src={record.mediaUrl} />
        </Fragment>
      ),
    },
    {
      title: '单价',
      dataIndex: 'productPrice',
      width: 150,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            onChange={e => {
              let index = this.state.products.indexOf(record);
              if (index >= 0) {
                let list = this.state.products.slice();
                list.splice(index, 1);
                record.productPrice = e;
                list.splice(index, 0, record);
                this.setState({ products: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '折扣总额',
      dataIndex: 'discountAmount',
      width: 150,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            onChange={e => {
              let index = this.state.products.indexOf(record);
              if (index >= 0) {
                let list = this.state.products.slice();
                list.splice(index, 1);
                record.discountAmount = e;
                list.splice(index, 0, record);
                this.setState({ products: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '数量',
      dataIndex: 'quantity',
      width: 150,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            precision={0}
            onChange={e => {
              let index = this.state.products.indexOf(record);
              if (index >= 0) {
                let list = this.state.products.slice();
                list.splice(index, 1);
                record.quantity = e;
                list.splice(index, 0, record);
                this.setState({ products: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '已发',
      dataIndex: 'shippedQuantity',
      width: 100,
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
            <Button
              onClick={() => this.handleRemoveProduct(record)}
              icon="close"
              type="danger"
              size="small"
            />
          </Button.Group>
        </Fragment>
      ),
    },
  ];

  componentDidMount() {
    if (this.state.id) this.handleInit(this.state.id);

    this.handleInitCountries();
  }

  handleTabChange = key => {
    if (key == 4) {
      //订单历史
      if (this.state.id) this.handleLoadHistory(this.state.id);
    }
  };

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
        if (!res.data.shippingAddress) res.data.shippingAddress = {};
        if (!res.data.billingAddress) res.data.billingAddress = {};
        this.setState({
          current: res.data,
          products: res.data.items,
        });
        if (res.data.shippingAddressId) {
          this.setState({ defaultShippingAddressId: res.data.shippingAddressId });
          this.handleInitProvinces(res.data.shippingAddress.countryId, 0);
        }
        if (res.data.billingAddressId) {
          this.setState({ defaultBillingAddressId: res.data.billingAddressId });
          this.handleInitProvinces(res.data.billingAddress.countryId, 1);
        }
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleRemoveProduct = record => {
    this.setState(({ products }) => {
      let index = products.indexOf(record);
      let list = products.slice();
      list.splice(index, 1);
      return {
        products: list,
      };
    });
  };

  handleChange = value => {
    var first = this.state.users.find(c => c.id == value);
    if (first) {
      // this.props.form.setFieldsValue({
      //     contactName: first.fullName || '',
      //     phone: first.phone || ''
      // })

      this.handleQueryUserAddresses(value);
    }
  };

  handleChangeShippingMethod = e => {
    this.setState({ shippingMethod: e.target.value });
  };

  handleQueryUserAddresses = userId => {
    const { dispatch } = this.props;
    this.setState({ userAddressesLoading: true });
    new Promise(resolve => {
      dispatch({
        type: 'system/userAddresses',
        payload: {
          resolve,
          params: { userId },
        },
      });
    }).then(res => {
      this.setState({ userAddressesLoading: false });
      if (res.success === true) {
        this.setState(
          {
            userAddresses: res.data,
            defaultShippingAddressId: res.data.defaultShippingAddressId || '',
            defaultBillingAddressId: res.data.defaultBillingAddressId || '',
          },
          () => {
            this.props.form.setFieldsValue({
              shippingAddressId: this.state.defaultShippingAddressId,
              billingAddressId: this.state.defaultBillingAddressId,
            });
          }
        );
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleQueryUsers = nameOrPhone => {
    const { dispatch } = this.props;
    this.setState({ usersLoading: true });
    new Promise(resolve => {
      dispatch({
        type: 'system/users',
        payload: {
          resolve,
          params: { nameOrPhone },
        },
      });
    }).then(res => {
      this.setState({ usersLoading: false });
      if (res.success === true) {
        this.setState({ users: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleInitCountries = () => {
    const { dispatch } = this.props;
    this.setState({ countriesLoading: true });
    new Promise(resolve => {
      dispatch({
        type: 'system/countries',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ countriesLoading: false });
      if (res.success === true) {
        this.setState({ countries: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleInitProvinces = (countryId, addressType) => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'system/provinces',
        payload: {
          resolve,
          params: { countryId: countryId },
        },
      });
    }).then(res => {
      if (res.success === true) {
        if (addressType == 0) {
          this.setState({ provinces: res.data });
        } else {
          this.setState({ provinces2: res.data });
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
        if (this.state.submitting) return;
        this.setState({ submitting: true });

        var params = {
          id: this.state.id,
          items: this.state.products,
          ...values,
        };

        //配送地址
        params.shippingAddressId = this.state.defaultShippingAddressId;
        if (params.shippingAddress) {
          var stateOrProvinceId =
            params.shippingAddress.stateOrProvinceId[
              params.shippingAddress.stateOrProvinceId.length - 1
            ];
          params.shippingAddress.stateOrProvinceId = stateOrProvinceId;
        }

        //账单地址
        params.billingAddressId = this.state.defaultBillingAddressId;
        if (params.billingAddress) {
          var stateOrProvinceId =
            params.billingAddress.stateOrProvinceId[
              params.billingAddress.stateOrProvinceId.length - 1
            ];
          params.billingAddress.stateOrProvinceId = stateOrProvinceId;
        }

        // console.log(params);

        // return;
        new Promise(resolve => {
          dispatch({
            type: 'order/edit',
            payload: {
              resolve,
              params,
            },
          });
        }).then(res => {
          this.setState(
            {
              submitting: false,
            },
            () => {
              if (res.success === true) {
                router.push('./list');
              } else {
                notification.error({ message: res.message });
              }
            }
          );
        });
      }
    });
  };

  saveFormRef = formRef => {
    this.formRef = formRef;
  };

  showProductModal = () => {
    this.setState({ visibleProduct: true }, () => {
      this.handleSearch();
    });
  };

  handleProductCancel = () => {
    this.setState({ visibleProduct: false });
  };

  handleSearch = name => {
    const { dispatch } = this.props;
    this.setState({ queryProductLoading: true });
    let params = {
      search: {
        name,
        isPublished: true,
      },
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
        notification.error({ message: res.message });
      }
    });
  };

  handleStandardTableChange = (pagination, filtersArg, sorter) => {
    this.setState(
      {
        pageNum: pagination.current,
        pageSize: pagination.pageSize,
      },
      () => {
        this.handleSearch();
      }
    );
  };

  onSelectChange = selectedRowKeys => {
    this.setState({
      visibleProduct: false,
      productsLoading: true,
    });
    let ids = [];
    ids = selectedRowKeys;
    if (!ids || ids.length <= 0) return;
    let pros = this.state.products;
    ids.forEach(id => {
      var old = pros.find(c => c.id == id);
      if (old) return;
      var first = this.state.pageData.list.find(c => c.id == id);
      if (first) {
        let pro = {
          id: first.id,
          name: first.name,
          productPrice: first.price,
          quantity: 1,
          mediaUrl: first.mediaUrl,
          discountAmount: 0,
        };
        pros.push(pro);
      }
    });
    this.setState({
      products: pros,
      productsLoading: false,
    });
  };

  render() {
    const {
      form: { getFieldDecorator, getFieldValue },
    } = this.props;
    const formItemLayout = {
      labelCol: {
        xs: { span: 24 },
        sm: { span: 4 },
      },
      wrapperCol: {
        xs: { span: 24 },
        sm: { span: 24 },
        md: { span: 20 },
      },
    };
    const submitFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 4 },
      },
    };
    const action = (
      <Fragment>
        <Button
          onClick={this.handleSubmit}
          type="primary"
          icon="save"
          htmlType="submit"
          loading={this.state.submitting}
        >
          保存
        </Button>
        <Link to="./list">
          <Button>
            <Icon type="rollback" />
          </Button>
        </Link>
      </Fragment>
    );
    const radioStyle = {
      display: 'block',
      height: '30px',
      lineHeight: '30px',
    };
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
      <PageHeaderWrapper title="订单 - 编辑" action={action}>
        <Spin spinning={this.state.loading}>
          <Card bordered={false}>
            <Form onSubmit={this.handleSubmit}>
              <Tabs type="card" onChange={this.handleTabChange}>
                <TabPane tab="基本信息" key="1">
                  <FormItem {...formItemLayout} label={<span>订单编号</span>}>
                    {this.state.current.no}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>客户</span>}>
                    <Input disabled value={this.state.current.customerName} />
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>配送方式</span>}>
                    {getFieldDecorator('shippingMethod', {
                      initialValue: this.state.current.shippingMethod || 0,
                    })(
                      <RadioGroup onChange={this.handleChangeShippingMethod}>
                        {ShippingMethod.map(x => (
                          <Radio key={x.key} value={x.key}>
                            {x.value}
                          </Radio>
                        ))}
                      </RadioGroup>
                    )}
                  </FormItem>
                  {this.state.shippingMethod == 1 ? (
                    <FormItem {...formItemLayout} label={<span>运费</span>}>
                      {getFieldDecorator('shippingFeeAmount', {
                        initialValue: this.state.current.shippingFeeAmount,
                        rules: [{ required: true, message: '请输入运费' }],
                      })(
                        <InputNumber
                          style={{ width: '100%' }}
                          min={0}
                          allowClear
                          placeholder="运费"
                        />
                      )}
                    </FormItem>
                  ) : null}
                  <FormItem {...formItemLayout} label={<span>付款类型</span>}>
                    {getFieldDecorator('paymentType', {
                      initialValue: this.state.current.paymentType || 0,
                    })(
                      <RadioGroup>
                        {PaymentType.map(x => (
                          <Radio key={x.key} value={x.key}>
                            {x.value}
                          </Radio>
                        ))}
                      </RadioGroup>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>付款方式</span>}>
                    {getFieldDecorator('paymentMethod', {
                      initialValue: this.state.current.paymentMethod,
                      // rules: [{ required: true, message: '配送方式' }],
                    })(
                      <Select
                        // mode="multiple"
                        allowClear
                        placeholder="付款方式"
                      >
                        {PaymentMethod.map(c => (
                          <Option key={c.key} value={c.key}>
                            {c.value}
                          </Option>
                        ))}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>付款金额</span>}>
                    {getFieldDecorator('paymentFeeAmount', {
                      initialValue: this.state.current.paymentFeeAmount,
                    })(
                      <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        allowClear
                        placeholder="付款金额"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>付款时间</span>}>
                    {getFieldDecorator('paymentOn', {
                      initialValue: this.state.current.paymentOn
                        ? moment(this.state.current.paymentOn, 'YYYY-MM-DD HH:mm:ss')
                        : null,
                    })(<DatePicker showTime format="YYYY-MM-DD HH:mm:ss" placeholder="付款时间" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>订单折扣</span>}>
                    {getFieldDecorator('discountAmount', {
                      initialValue: this.state.current.discountAmount,
                      rules: [{ required: true, message: '请输入订单折扣金额' }],
                    })(
                      <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        allowClear
                        placeholder="折扣金额"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>订单总额</span>}>
                    {getFieldDecorator('orderTotal', {
                      initialValue: this.state.current.orderTotal,
                      rules: [{ required: true, message: '请输入订单总额' }],
                    })(
                      <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        allowClear
                        placeholder="订单总额"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>订单状态</span>}>
                    {getFieldDecorator('orderStatus', {
                      initialValue: this.state.current.orderStatus,
                      rules: [{ required: true, message: '请选择订单状态' }],
                    })(
                      <Select
                        // mode="multiple"
                        // allowClear
                        placeholder="订单状态"
                      >
                        {OrderStatus.map(c => (
                          <Option key={c.key} value={c.key}>
                            {c.value}
                          </Option>
                        ))}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>配送状态</span>}>
                    {getFieldDecorator('shippingStatus', {
                      initialValue: this.state.current.shippingStatus,
                      // rules: [{ required: true, message: '配送方式' }],
                    })(
                      <Select
                        // mode="multiple"
                        allowClear
                        placeholder="配送状态"
                      >
                        {ShippingStatus.map(c => (
                          <Option key={c.key} value={c.key}>
                            {c.value}
                          </Option>
                        ))}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>发货时间</span>}>
                    {getFieldDecorator('shippedOn', {
                      initialValue: this.state.current.shippedOn
                        ? moment(this.state.current.shippedOn, 'YYYY-MM-DD HH:mm:ss')
                        : null,
                    })(<DatePicker showTime format="YYYY-MM-DD HH:mm:ss" placeholder="发货时间" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>收货时间</span>}>
                    {getFieldDecorator('deliveredOn', {
                      initialValue: this.state.current.deliveredOn
                        ? moment(this.state.current.deliveredOn, 'YYYY-MM-DD HH:mm:ss')
                        : null,
                    })(<DatePicker showTime format="YYYY-MM-DD HH:mm:ss" placeholder="收货时间" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>交易取消时间</span>}>
                    {getFieldDecorator('cancelOn', {
                      initialValue: this.state.current.cancelOn
                        ? moment(this.state.current.cancelOn, 'YYYY-MM-DD HH:mm:ss')
                        : null,
                    })(
                      <DatePicker
                        showTime
                        format="YYYY-MM-DD HH:mm:ss"
                        placeholder="交易取消时间"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>交易取消原因</span>}>
                    {getFieldDecorator('cancelReason', {
                      initialValue: this.state.current.cancelReason || '',
                    })(
                      <TextArea
                        style={{ minHeight: 32 }}
                        placeholder="交易关闭/交易取消原因"
                        rows={2}
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>下单备注</span>}>
                    {getFieldDecorator('orderNote', {
                      initialValue: this.state.current.orderNote || '',
                    })(<TextArea style={{ minHeight: 32 }} placeholder="" rows={2} />)}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={<span style={{ color: 'red' }}>退款状态</span>}
                  >
                    {getFieldDecorator('refundStatus', {
                      initialValue: this.state.current.refundStatus,
                    })(
                      <Select allowClear placeholder="配送状态">
                        {RefundStatus.map(c => (
                          <Option key={c.key} value={c.key}>
                            {c.value}
                          </Option>
                        ))}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={<span style={{ color: 'red' }}>退款金额</span>}
                  >
                    {getFieldDecorator('refundAmount', {
                      initialValue: this.state.current.refundAmount,
                    })(
                      <InputNumber
                        style={{ width: '100%' }}
                        min={0}
                        allowClear
                        placeholder="退款金额"
                      />
                    )}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={<span style={{ color: 'red' }}>退款时间</span>}
                  >
                    {getFieldDecorator('refundOn', {
                      initialValue: this.state.current.refundOn
                        ? moment(this.state.current.refundOn, 'YYYY-MM-DD HH:mm:ss')
                        : null,
                    })(<DatePicker showTime format="YYYY-MM-DD HH:mm:ss" placeholder="退款时间" />)}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={<span style={{ color: 'red' }}>退款原因</span>}
                  >
                    {getFieldDecorator('refundReason', {
                      initialValue: this.state.current.refundReason || '',
                    })(<TextArea style={{ minHeight: 32 }} placeholder="退款原因" rows={2} />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>管理员备注</span>}>
                    {getFieldDecorator('adminNote', {
                      initialValue: this.state.current.adminNote || '',
                    })(<TextArea style={{ minHeight: 32 }} placeholder="" rows={2} />)}
                  </FormItem>
                </TabPane>
                <TabPane tab="配送 & 账单" key="2">
                  <Card type="inner" title="配送地址">
                    <FormItem>
                      {getFieldDecorator('shippingAddressId', {
                        initialValue: this.state.defaultShippingAddressId,
                      })(
                        <RadioGroup
                          onChange={e => {
                            this.setState({ defaultShippingAddressId: e.target.value });
                          }}
                        >
                          <Radio style={radioStyle} value={''}>
                            无
                          </Radio>
                          {this.state.current.shippingAddressId ? (
                            <Radio
                              key={this.state.current.shippingAddressId}
                              style={radioStyle}
                              value={this.state.current.shippingAddressId}
                            >
                              配送地址
                            </Radio>
                          ) : (
                            <Radio style={radioStyle} value={0}>
                              新地址
                            </Radio>
                          )}
                        </RadioGroup>
                      )}
                    </FormItem>
                    {this.state.defaultShippingAddressId ||
                    this.state.defaultShippingAddressId === 0 ? (
                      <div>
                        <FormItem {...formItemLayout} label={<span>联系人</span>}>
                          {getFieldDecorator('shippingAddress.contactName', {
                            initialValue: this.state.current.shippingAddress.contactName,
                            rules: [{ required: true, message: '请输入联系人' }],
                          })(<Input allowClear placeholder="客户名称" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>手机</span>}>
                          {getFieldDecorator('shippingAddress.phone', {
                            initialValue: this.state.current.shippingAddress.phone,
                            rules: [{ required: true, message: '请输入联系方式' }],
                          })(<Input allowClear placeholder="电话/手机" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>邮箱</span>}>
                          {getFieldDecorator('shippingAddress.email', {
                            initialValue: this.state.current.shippingAddress.email,
                          })(<Input placeholder="邮箱" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>国家</span>}>
                          {getFieldDecorator('shippingAddress.countryId', {
                            initialValue: this.state.current.shippingAddress.countryId,
                            rules: [{ required: true, message: '请选择国家' }],
                          })(
                            <Select
                              showSearch
                              placeholder="Select a country"
                              optionFilterProp="children"
                              onChange={value => {
                                this.handleInitProvinces(value, 0);
                              }}
                              filterOption={(input, option) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >=
                                0
                              }
                            >
                              {this.state.countries.map(d => (
                                <Option key={d.id} value={d.id}>
                                  {d.name}
                                </Option>
                              ))}
                            </Select>
                          )}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>省市区</span>}>
                          {getFieldDecorator('shippingAddress.stateOrProvinceId', {
                            initialValue: this.state.current.shippingAddress.stateOrProvinceIds,
                            rules: [{ required: true, message: '请选择省市区' }],
                          })(
                            <Cascader
                              changeOnSelect
                              options={this.state.provinces}
                              placeholder="Please select"
                            />
                          )}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>城市</span>}>
                          {getFieldDecorator('shippingAddress.city', {
                            initialValue: this.state.current.shippingAddress.city,
                          })(<Input placeholder="城市" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>邮编</span>}>
                          {getFieldDecorator('shippingAddress.zipCode', {
                            initialValue: this.state.current.shippingAddress.zipCode,
                          })(<Input placeholder="邮政编码" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>公司</span>}>
                          {getFieldDecorator('shippingAddress.company', {
                            initialValue: this.state.current.shippingAddress.company,
                          })(<Input placeholder="公司" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>街道地址</span>}>
                          {getFieldDecorator('shippingAddress.addressLine1', {
                            initialValue: this.state.current.shippingAddress.addressLine1,
                            rules: [{ required: true, message: '请输入街道地址' }],
                          })(<Input placeholder="街道地址" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>街道地址2</span>}>
                          {getFieldDecorator('shippingAddress.addressLine2', {
                            initialValue: this.state.current.shippingAddress.addressLine2,
                          })(<Input placeholder="街道地址2" />)}
                        </FormItem>
                      </div>
                    ) : null}
                  </Card>
                  <Card type="inner" title="账单地址" style={{ marginTop: 16 }}>
                    <FormItem>
                      {getFieldDecorator('billingAddressId', {
                        initialValue: this.state.defaultBillingAddressId,
                      })(
                        <RadioGroup
                          onChange={e => {
                            this.setState({ defaultBillingAddressId: e.target.value });
                          }}
                        >
                          <Radio style={radioStyle} value={''}>
                            无
                          </Radio>
                          {this.state.current.billingAddressId ? (
                            <Radio
                              key={this.state.current.billingAddressId}
                              style={radioStyle}
                              value={this.state.current.billingAddressId}
                            >
                              账单地址
                            </Radio>
                          ) : (
                            <Radio style={radioStyle} value={0}>
                              新地址
                            </Radio>
                          )}
                        </RadioGroup>
                      )}
                    </FormItem>
                    {this.state.defaultBillingAddressId === 0 ||
                    this.state.defaultBillingAddressId ? (
                      <div>
                        <FormItem {...formItemLayout} label={<span>联系人</span>}>
                          {getFieldDecorator('billingAddress.contactName', {
                            initialValue: this.state.current.billingAddress.contactName || '',
                            rules: [{ required: true, message: '请输入联系人' }],
                          })(<Input allowClear placeholder="客户名称" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>手机</span>}>
                          {getFieldDecorator('billingAddress.phone', {
                            initialValue: this.state.current.billingAddress.phone,
                            rules: [{ required: true, message: '请输入联系方式' }],
                          })(<Input allowClear placeholder="电话/手机" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>邮箱</span>}>
                          {getFieldDecorator('billingAddress.email', {
                            initialValue: this.state.current.billingAddress.email,
                          })(<Input placeholder="邮箱" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>国家</span>}>
                          {getFieldDecorator('billingAddress.countryId', {
                            initialValue: this.state.current.billingAddress.countryId,
                            rules: [{ required: true, message: '请选择国家' }],
                          })(
                            <Select
                              showSearch
                              placeholder="Select a country"
                              optionFilterProp="children"
                              onChange={value => {
                                this.handleInitProvinces(value, 1);
                              }}
                              filterOption={(input, option) =>
                                option.props.children.toLowerCase().indexOf(input.toLowerCase()) >=
                                0
                              }
                            >
                              {this.state.countries.map(d => (
                                <Option key={d.id} value={d.id}>
                                  {d.name}
                                </Option>
                              ))}
                            </Select>
                          )}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>省市区</span>}>
                          {getFieldDecorator('billingAddress.stateOrProvinceId', {
                            initialValue: this.state.current.billingAddress.stateOrProvinceIds,
                            rules: [{ required: true, message: '请选择省市区' }],
                          })(
                            <Cascader
                              changeOnSelect
                              options={this.state.provinces2}
                              placeholder="Please select"
                            />
                          )}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>城市</span>}>
                          {getFieldDecorator('billingAddress.city', {
                            initialValue: this.state.current.billingAddress.city,
                          })(<Input placeholder="城市" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>邮编</span>}>
                          {getFieldDecorator('billingAddress.zipCode', {
                            initialValue: this.state.current.billingAddress.zipCode,
                          })(<Input placeholder="邮政编码" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>公司</span>}>
                          {getFieldDecorator('billingAddress.company', {
                            initialValue: this.state.current.billingAddress.company,
                          })(<Input placeholder="公司" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>街道地址</span>}>
                          {getFieldDecorator('billingAddress.addressLine1', {
                            initialValue: this.state.current.billingAddress.addressLine1,
                            rules: [{ required: true, message: '请输入街道地址' }],
                          })(<Input placeholder="街道地址" />)}
                        </FormItem>
                        <FormItem {...formItemLayout} label={<span>街道地址2</span>}>
                          {getFieldDecorator('billingAddress.addressLine2', {
                            initialValue: this.state.current.billingAddress.addressLine2,
                          })(<Input placeholder="街道地址2" />)}
                        </FormItem>
                      </div>
                    ) : null}
                  </Card>
                </TabPane>
                <TabPane tab="商品信息" key="3">
                  <Button
                    icon="plus"
                    type="primary"
                    style={{ marginBottom: 16 }}
                    onClick={this.showProductModal}
                  >
                    添加商品
                  </Button>
                  <Table
                    bordered={false}
                    rowKey={(record, index) => `product_${record.id}_i_${index}`} //{record => record.id}
                    pagination={false}
                    loading={this.state.productsLoading}
                    dataSource={this.state.products}
                    columns={this.columnsProduct}
                    // scroll={{ x: 960 }}
                  />
                  <div style={{ marginTop: 12 }}>
                    商品总数：
                    {eval(this.state.products.map(x => parseInt(x.quantity)).join('+'))}
                  </div>
                  <div>
                    折扣小计：
                    <span style={{ color: '#52c41a', fontWeight: 'bold' }}>
                      {eval(this.state.products.map(x => parseFloat(x.discountAmount)).join('+'))}
                    </span>
                  </div>
                  <div>
                    总额小计：
                    {this.state.products && this.state.products.length > 0 ? (
                      <span style={{ color: 'red', fontWeight: 'bold' }}>
                        {eval(
                          this.state.products
                            .map(x => parseInt(x.quantity) * parseFloat(x.productPrice))
                            .join('+')
                        ) -
                          eval(
                            this.state.products.map(x => parseFloat(x.discountAmount)).join('+')
                          )}
                      </span>
                    ) : (
                      0
                    )}
                  </div>
                </TabPane>
                <TabPane tab="操作记录" key="4">
                  <Table
                    bordered={false}
                    rowKey={(record, index) => `order_history_${record.id}_i_${index}`} //{record => record.id}
                    pagination={false}
                    loading={this.state.historiesLoading}
                    dataSource={this.state.histories}
                    columns={this.columnsHistories}
                  />
                </TabPane>
              </Tabs>
            </Form>
          </Card>
          <ProductCommponent
            pagination={pagination}
            pageData={this.state.pageData}
            visible={this.state.visibleProduct}
            loading={this.state.queryProductLoading}
            wrappedComponentRef={this.saveFormRef}
            handleSearch={this.handleSearch}
            onCancel={this.handleProductCancel}
            onChange={this.handleStandardTableChange}
            onOk={val => {
              this.onSelectChange(val);
            }}
          />
        </Spin>
      </PageHeaderWrapper>
    );
  }
}

export default EditOrder;
