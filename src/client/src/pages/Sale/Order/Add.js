import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import {
    Form, Input, Button, Card, InputNumber, Icon, Checkbox, notification, Select, Spin,
    Table, Tabs, Cascader, Radio, Avatar
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

@connect()
@Form.create()
class AddOrder extends PureComponent {
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
                addresses: []
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
            title: '单价',
            dataIndex: 'productPrice',
            width: 150,
            render: (text, record) => (
                <Fragment>
                    <InputNumber
                        min={0}
                        onChange={(e) => {
                            let index = this.state.products.indexOf(record);
                            if (index >= 0) {
                                let list = this.state.products.slice();
                                list.splice(index, 1);
                                record.productPrice = e;
                                list.splice(index, 0, record);
                                this.setState({ products: list });
                            }
                        }}
                        defaultValue={text}></InputNumber>
                </Fragment>
            )
        },
        {
            title: '折扣总额',
            dataIndex: 'discountAmount',
            width: 150,
            render: (text, record) => (
                <Fragment>
                    <InputNumber
                        min={0}
                        onChange={(e) => {
                            let index = this.state.products.indexOf(record);
                            if (index >= 0) {
                                let list = this.state.products.slice();
                                list.splice(index, 1);
                                record.discountAmount = e;
                                list.splice(index, 0, record);
                                this.setState({ products: list });
                            }
                        }}
                        defaultValue={text}></InputNumber>
                </Fragment>
            )
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
                        onChange={(e) => {
                            let index = this.state.products.indexOf(record);
                            if (index >= 0) {
                                let list = this.state.products.slice();
                                list.splice(index, 1);
                                record.quantity = e;
                                list.splice(index, 0, record);
                                this.setState({ products: list });
                            }
                        }}
                        defaultValue={text}></InputNumber>
                </Fragment>
            )
        },
        {
            title: '图片',
            dataIndex: 'mediaUrl',
            align: 'center',
            width: 64,
            fixed: 'right',
            render: (text, record) => (
                <Fragment>
                    <Avatar shape="square" size={32} src={record.mediaUrl} />
                </Fragment>
            )
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
        this.handleInitCountries();
    }

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

    handleChange = (value) => {
        var first = this.state.users.find(c => c.id == value);
        if (first) {
            // this.props.form.setFieldsValue({
            //     contactName: first.fullName || '',
            //     phone: first.phone || ''
            // })

            this.handleQueryUserAddresses(value);
        }
    }

    handleChangeShippingMethod = (e) => {
        this.setState({ shippingMethod: e.target.value });
    }

    handleQueryUserAddresses = (userId) => {
        const { dispatch } = this.props;
        this.setState({ userAddressesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'system/userAddresses',
                payload: {
                    resolve,
                    params: { userId }
                },
            });
        }).then(res => {
            this.setState({ userAddressesLoading: false });
            if (res.success === true) {
                this.setState({
                    userAddresses: res.data,
                    defaultShippingAddressId: res.data.defaultShippingAddressId || '',
                    defaultBillingAddressId: res.data.defaultBillingAddressId || '',
                }, () => {
                    this.props.form.setFieldsValue({
                        shippingUserAddressId: this.state.defaultShippingAddressId,
                        billingUserAddressId: this.state.defaultBillingAddressId
                    })
                });
            } else {
                notification.error({ message: res.message });
            }
        });
    }

    handleQueryUsers = (nameOrPhone) => {
        const { dispatch } = this.props;
        this.setState({ usersLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'system/users',
                payload: {
                    resolve,
                    params: { nameOrPhone }
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
    }

    handleInitCountries = () => {
        const { dispatch } = this.props;
        this.setState({ countriesLoading: true });
        new Promise(resolve => {
            dispatch({
                type: 'system/countries',
                payload: {
                    resolve
                },
            });
        }).then(res => {
            this.setState({ countriesLoading: false });
            if (res.success === true) {
                this.setState({ countries: res.data });
            } else {
                notification.error({ message: res.message, });
            }
        });
    }

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
                notification.error({ message: res.message, });
            }
        });
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
                    items: this.state.products,
                    ...values
                };

                //配送地址
                if (params.shippingAddress) {
                    var stateOrProvinceId = params.shippingAddress.stateOrProvinceId[params.shippingAddress.stateOrProvinceId.length - 1];
                    params.shippingAddress.stateOrProvinceId = stateOrProvinceId;
                }

                //账单地址
                if (params.billingAddress) {
                    var stateOrProvinceId = params.billingAddress.stateOrProvinceId[params.billingAddress.stateOrProvinceId.length - 1];
                    params.billingAddress.stateOrProvinceId = stateOrProvinceId;
                }

                // console.log(params);
                // return;
                new Promise(resolve => {
                    dispatch({
                        type: 'order/add',
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
                isPublished: true
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
                    productPrice: first.price,
                    quantity: 1,
                    mediaUrl: first.mediaUrl,
                    discountAmount: 0
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
                <Button onClick={this.handleSubmit} type="primary" icon="save" htmlType="submit" loading={this.state.submitting}>
                    保存</Button>
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
            }
        };

        return (
            <PageHeaderWrapper title="订单 - 添加" action={action}>
                <Card bordered={false}>
                    <Form onSubmit={this.handleSubmit}>
                        <Tabs type="card" onChange={this.handleTabChange}>
                            <TabPane tab="基本信息" key="1">
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>客户</span>}>
                                    {getFieldDecorator('customerId', {
                                        initialValue: this.state.current.customerId,
                                        rules: [{ required: true, message: '请选择客户' }],
                                    })(<Select
                                        // mode="multiple"
                                        // style={{ width: '100%' }}
                                        // labelInValue
                                        allowClear
                                        showSearch
                                        placeholder="请输入客户名称或联系方式"
                                        notFoundContent={this.state.usersLoading ? <Spin size="small" /> : null}
                                        filterOption={false}
                                        onSearch={this.handleQueryUsers}
                                        onChange={this.handleChange}
                                    >
                                        {this.state.users.map(d =>
                                            <Option key={d.id} value={d.id}>{d.fullName}</Option>)}
                                    </Select>)}
                                </FormItem>
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>配送方式</span>}>
                                    {getFieldDecorator('shippingMethod',
                                        { initialValue: this.state.current.shippingMethod || 0 })(
                                            <RadioGroup onChange={this.handleChangeShippingMethod}>
                                                {
                                                    ShippingMethod.map(x =>
                                                        <Radio key={x.key} value={x.key}>{x.value}</Radio>)
                                                }
                                            </RadioGroup>)
                                    }
                                </FormItem>
                                {
                                    this.state.shippingMethod == 1 ?
                                        <FormItem
                                            {...formItemLayout}
                                            label={<span>运费</span>}>
                                            {getFieldDecorator('shippingFeeAmount', {
                                                initialValue: this.state.current.shippingFeeAmount,
                                                rules: [{ required: true, message: '请输入运费' }],
                                            })(<InputNumber style={{ width: '100%' }} min={0} allowClear placeholder="运费" />)}
                                        </FormItem> : null
                                }
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>付款类型</span>}>
                                    {getFieldDecorator('paymentType',
                                        { initialValue: this.state.current.paymentType || 0 })(
                                            <RadioGroup>
                                                {
                                                    PaymentType.map(x =>
                                                        <Radio key={x.key} value={x.key}>{x.value}</Radio>)
                                                }
                                            </RadioGroup>)
                                    }
                                </FormItem>
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>订单折扣</span>}>
                                    {getFieldDecorator('discountAmount', {
                                        initialValue: this.state.current.discountAmount,
                                        rules: [{ required: true, message: '请输入订单折扣金额' }],
                                    })(<InputNumber style={{ width: '100%' }} min={0} allowClear placeholder="折扣金额" />)}
                                </FormItem>
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>订单总额</span>}>
                                    {getFieldDecorator('orderTotal', {
                                        initialValue: this.state.current.orderTotal,
                                        rules: [{ required: true, message: '请输入订单总额' }],
                                    })(<InputNumber style={{ width: '100%' }} min={0} allowClear placeholder="订单总额" />)}
                                </FormItem>
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>下单备注</span>}>
                                    {getFieldDecorator('orderNote', { initialValue: this.state.current.orderNote || '' })(
                                        <TextArea
                                            style={{ minHeight: 32 }}
                                            placeholder=""
                                            rows={2} />)
                                    }
                                </FormItem>
                                <FormItem
                                    {...formItemLayout}
                                    label={<span>管理员备注</span>}>
                                    {getFieldDecorator('adminNote', { initialValue: this.state.current.adminNote || '' })(
                                        <TextArea
                                            style={{ minHeight: 32 }}
                                            placeholder=""
                                            rows={2} />)
                                    }
                                </FormItem>
                            </TabPane>
                            <TabPane tab="配送 & 账单" key="2">
                                <Card type="inner" title="配送地址">
                                    <FormItem>
                                        {getFieldDecorator('shippingUserAddressId',
                                            { initialValue: this.state.defaultShippingAddressId })(
                                                <RadioGroup onChange={(e) => {
                                                    this.setState({ defaultShippingAddressId: e.target.value });
                                                }}>
                                                    <Radio style={radioStyle} value={''}>无</Radio>
                                                    {
                                                        this.state.userAddresses.addresses.filter(c => c.addressType == 0).map(x =>
                                                            <Radio key={x.userAddressId} style={radioStyle} value={x.userAddressId}>{
                                                                `${x.countryName}, ${x.stateOrProvinceName}, ${x.cityName || ''}, ${x.addressLine1 || ''} (${x.contactName}, ${x.phone || ''})`
                                                            }</Radio>)
                                                    }
                                                    <Radio style={radioStyle} value={0}>添加地址</Radio>
                                                </RadioGroup>)
                                        }
                                    </FormItem>
                                    {
                                        this.state.defaultShippingAddressId === 0 ?
                                            <div>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>联系人</span>}>
                                                    {getFieldDecorator('shippingAddress.contactName', {
                                                        // initialValue: this.state.current.shippingAddress.contactName || '',
                                                        rules: [{ required: true, message: '请输入联系人' }],
                                                    })(<Input allowClear placeholder="客户名称" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>手机</span>}>
                                                    {getFieldDecorator('shippingAddress.phone', {
                                                        // initialValue: this.state.current.shippingAddress.phone || '',
                                                        rules: [{ required: true, message: '请输入联系方式' }],
                                                    })(<Input allowClear placeholder="电话/手机" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>邮箱</span>}>
                                                    {getFieldDecorator('shippingAddress.email', {
                                                        // initialValue: this.state.current.shippingAddress.email || '',
                                                    })(<Input placeholder="邮箱" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>国家</span>}>
                                                    {getFieldDecorator('shippingAddress.countryId', {
                                                        // initialValue: this.state.current.shippingAddress.countryId || '',
                                                        rules: [{ required: true, message: '请选择国家' }],
                                                    })(<Select
                                                        showSearch
                                                        placeholder="Select a country"
                                                        optionFilterProp="children"
                                                        onChange={(value) => {
                                                            this.handleInitProvinces(value, 0);
                                                        }}
                                                        filterOption={(input, option) => option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0}
                                                    >
                                                        {this.state.countries.map(d =>
                                                            <Option key={d.id} value={d.id}>{d.name}</Option>)}
                                                    </Select>)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>省市区</span>}>
                                                    {getFieldDecorator('shippingAddress.stateOrProvinceId', {
                                                        // initialValue: this.state.current.shippingAddress.stateOrProvinceIds || [],
                                                        rules: [{ required: true, message: '请选择省市区' }],
                                                    })(<Cascader changeOnSelect options={this.state.provinces} placeholder="Please select" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>城市</span>}>
                                                    {getFieldDecorator('shippingAddress.city', {
                                                        // initialValue: this.state.current.shippingAddress.city || '',
                                                    })(<Input placeholder="城市" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>邮编</span>}>
                                                    {getFieldDecorator('shippingAddress.zipCode', {
                                                        // initialValue: this.state.current.shippingAddress.zipCode || '',
                                                    })(<Input placeholder="邮政编码" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>公司</span>}>
                                                    {getFieldDecorator('shippingAddress.company', {
                                                        // initialValue: this.state.current.shippingAddress.company || '',
                                                    })(<Input placeholder="公司" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>街道地址</span>}>
                                                    {getFieldDecorator('shippingAddress.addressLine1', {
                                                        // initialValue: this.state.current.shippingAddress.addressLine1 || '',
                                                        rules: [{ required: true, message: '请输入街道地址' }],
                                                    })(<Input placeholder="街道地址" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>街道地址2</span>}>
                                                    {getFieldDecorator('shippingAddress.addressLine2', {
                                                        // initialValue: this.state.current.shippingAddress.addressLine2 || '',
                                                    })(<Input placeholder="街道地址2" />)}
                                                </FormItem>
                                            </div> : null
                                    }
                                </Card>
                                <Card type="inner" title="账单地址"
                                    style={{ marginTop: 16 }}>
                                    <FormItem>
                                        {getFieldDecorator('billingUserAddressId',
                                            { initialValue: this.state.defaultBillingAddressId })(
                                                <RadioGroup onChange={(e) => {
                                                    this.setState({ defaultBillingAddressId: e.target.value });
                                                }}>
                                                    <Radio style={radioStyle} value={''}>无</Radio>
                                                    {
                                                        this.state.userAddresses.addresses.filter(c => c.addressType == 1).map(x =>
                                                            <Radio key={x.userAddressId} style={radioStyle} value={x.userAddressId}>{
                                                                `${x.countryName}, ${x.stateOrProvinceName}, ${x.cityName || ''}, ${x.addressLine1 || ''} (${x.contactName}, ${x.phone || ''})`
                                                            }</Radio>)
                                                    }
                                                    <Radio style={radioStyle} value={0}>添加地址</Radio>
                                                </RadioGroup>)
                                        }
                                    </FormItem>
                                    {
                                        this.state.defaultBillingAddressId === 0 ?
                                            <div>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>联系人</span>}>
                                                    {getFieldDecorator('billingAddress.contactName', {
                                                        // initialValue: this.state.current.shippingAddress.contactName || '',
                                                        rules: [{ required: true, message: '请输入联系人' }],
                                                    })(<Input allowClear placeholder="客户名称" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>手机</span>}>
                                                    {getFieldDecorator('billingAddress.phone', {
                                                        // initialValue: this.state.current.shippingAddress.phone || '',
                                                        rules: [{ required: true, message: '请输入联系方式' }],
                                                    })(<Input allowClear placeholder="电话/手机" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>邮箱</span>}>
                                                    {getFieldDecorator('billingAddress.email', {
                                                        // initialValue: this.state.current.shippingAddress.email || '',
                                                    })(<Input placeholder="邮箱" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>国家</span>}>
                                                    {getFieldDecorator('billingAddress.countryId', {
                                                        // initialValue: this.state.current.shippingAddress.countryId || '',
                                                        rules: [{ required: true, message: '请选择国家' }],
                                                    })(<Select
                                                        showSearch
                                                        placeholder="Select a country"
                                                        optionFilterProp="children"
                                                        onChange={(value) => {
                                                            this.handleInitProvinces(value, 0);
                                                        }}
                                                        filterOption={(input, option) => option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0}
                                                    >
                                                        {this.state.countries.map(d =>
                                                            <Option key={d.id} value={d.id}>{d.name}</Option>)}
                                                    </Select>)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>省市区</span>}>
                                                    {getFieldDecorator('billingAddress.stateOrProvinceId', {
                                                        // initialValue: this.state.current.shippingAddress.stateOrProvinceIds || [],
                                                        rules: [{ required: true, message: '请选择省市区' }],
                                                    })(<Cascader changeOnSelect options={this.state.provinces} placeholder="Please select" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>城市</span>}>
                                                    {getFieldDecorator('billingAddress.city', {
                                                        // initialValue: this.state.current.shippingAddress.city || '',
                                                    })(<Input placeholder="城市" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>邮编</span>}>
                                                    {getFieldDecorator('billingAddress.zipCode', {
                                                        // initialValue: this.state.current.shippingAddress.zipCode || '',
                                                    })(<Input placeholder="邮政编码" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>公司</span>}>
                                                    {getFieldDecorator('billingAddress.company', {
                                                        // initialValue: this.state.current.shippingAddress.company || '',
                                                    })(<Input placeholder="公司" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>街道地址</span>}>
                                                    {getFieldDecorator('billingAddress.addressLine1', {
                                                        // initialValue: this.state.current.shippingAddress.addressLine1 || '',
                                                        rules: [{ required: true, message: '请输入街道地址' }],
                                                    })(<Input placeholder="街道地址" />)}
                                                </FormItem>
                                                <FormItem
                                                    {...formItemLayout}
                                                    label={<span>街道地址2</span>}>
                                                    {getFieldDecorator('billingAddress.addressLine2', {
                                                        // initialValue: this.state.current.shippingAddress.addressLine2 || '',
                                                    })(<Input placeholder="街道地址2" />)}
                                                </FormItem>
                                            </div> : null
                                    }
                                </Card>


                            </TabPane>
                            <TabPane tab="商品信息" key="3">
                                <Button icon="plus" type="primary" style={{ marginBottom: 16 }} onClick={this.showProductModal}>添加商品</Button>
                                <Table bordered={false}
                                    rowKey={(record, index) => `product_${record.id}_i_${index}`} //{record => record.id}
                                    pagination={false}
                                    loading={this.state.productsLoading}
                                    dataSource={this.state.products}
                                    columns={this.columnsProduct}
                                // scroll={{ x: 960 }}
                                />
                                <div style={{ marginTop: 12 }}>商品总数：{eval(this.state.products.map(x => parseInt(x.quantity)).join('+'))}</div>
                                <div>折扣小计：
                                    <span style={{ color: '#52c41a', fontWeight: 'bold' }}>
                                        {eval(this.state.products.map(x => parseFloat(x.discountAmount)).join('+'))}</span></div>
                                <div>总额小计：
                                    {
                                        this.state.products && this.state.products.length > 0 ?
                                            <span style={{ color: 'red', fontWeight: 'bold' }}>{eval(this.state.products.map(x => parseInt(x.quantity) * parseFloat(x.productPrice)).join('+')) - eval(this.state.products.map(x => parseFloat(x.discountAmount)).join('+'))}</span>
                                            : 0
                                    }
                                </div>
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
                    onOk={(val) => { this.onSelectChange(val) }}
                />
            </PageHeaderWrapper>
        );
    }
}

export default AddOrder;
