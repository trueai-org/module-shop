import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    List, Card, Input, Button, Modal, Form, notification, Table, Popconfirm, Divider, Select, Tag, Icon,
    Redio, Menu, Dropdown, Switch,
    Row, Col, InputNumber, DatePicker, Checkbox
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';

import router from 'umi/router';
import Link from 'umi/link';

import styles from './List.less';

const FormItem = Form.Item;
const Option = Select.Option;

@connect()
@Form.create()
class ProductList extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            visible: false,
            data: [],
            current: {},
            submitting: false,

            children: [],

            pageNum: 1,
            pageSize: 10,
            predicate: 'id',
            reverse: true,
            pageData: {
                list: [],
                pagination: {}
            },

            expandForm: false,
            queryParam: {},

            categoryLoading: false, //类别加载中
            categories: [],
        };
    }

    columns = [
        {
            title: '操作',
            key: 'operation',
            fixed: 'left',
            align: 'center',
            width: 135,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button icon="edit" size="small" onClick={() => this.handleEdit(record.id)}></Button>
                        <Button style={{ color: record.isPublished == true ? "#f5222d" : "#1890ff" }} icon={record.isPublished == true ? "pause-circle" : "play-circle"} size="small"
                            onClick={() => this.handlePublish(record.id, !record.isPublished)}></Button>
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.deleteItem(record.id)}>
                            <Button icon="delete" type="danger" size="small"></Button>
                        </Popconfirm>
                    </Button.Group>
                </Fragment>
            )
        },
        {
            title: 'ID',
            dataIndex: 'id',
            // fixed: 'left',
            sorter: true,
            defaultSortOrder: 'descend',
            width: 100,
        },
        {
            title: '名称',
            dataIndex: 'name',
            sorter: true,
            // render: (text, record) => (
            //     <Fragment>
            //         {text}
            //         {record.isFeatured ? <Tag color="red">精品</Tag> : null}
            //     </Fragment>
            // )
        },
        {
            title: '价格/￥',
            dataIndex: 'price',
            sorter: true,
            width: 120,
        },
        {
            title: '是否发布',
            dataIndex: 'isPublished',
            sorter: true,
            width: 120,
            align: 'center',
            render: (val) => this.boolFormat(val)
        },
        {
            title: '允许订购',
            dataIndex: 'isAllowToOrder',
            sorter: true,
            width: 120,
            align: 'center',
            render: (val) => this.boolFormat(val)
        },
        {
            title: '有选项',
            dataIndex: 'hasOptions',
            sorter: true,
            width: 120,
            align: 'center',
            render: (val) => this.boolFormat(val)
        },
        {
            title: '单独可见',
            dataIndex: 'isVisibleIndividually',
            sorter: true,
            width: 120,
            align: 'center',
            render: (val) => this.boolFormat(val)
        },
        {
            title: '精品',
            dataIndex: 'isFeatured',
            sorter: true,
            width: 120,
            align: 'center',
            render: (val) => this.boolFormat(val)
        },
        {
            title: '库存',
            dataIndex: 'stockQuantity',
            sorter: true,
            width: 100,
        },
        {
            title: '创建时间',
            dataIndex: 'createdOn',
            sorter: true,
            width: 120,
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },
        {
            title: '更新时间',
            dataIndex: 'updatedOn',
            sorter: true,
            width: 120,
            render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        }
    ];


    boolFormat(val) {
        //(val) => <Switch checked={val} disabled />,
        return <Icon style={{ color: val == true ? "#1890ff" : "#f5222d" }} type={val == true ? "check" : "close"} />;
    }

    componentDidMount() {
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'catalog/categories',
                payload: {
                    resolve,
                },
            });
        }).then(res => {
            this.setState({ categoryLoading: false });
            if (res.success === true) {
                this.setState({ categories: res.data });
            } else {
                notification.error({ message: res.message });
            }
        });

        this.queryData(this.state.queryParam);
    }

    deleteItem = id => {
        this.setState({
            loading: true,
        });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: 'product/delete',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({
                loading: false,
            });
            if (res.success === true) {
                this.queryData();
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    handlePublish = (id, isPublish) => {
        this.setState({
            loading: true,
        });
        const { dispatch } = this.props;
        const params = { id };
        new Promise(resolve => {
            dispatch({
                type: isPublish ? 'product/publish' : 'product/unpublish',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({
                loading: false,
            });
            if (res.success === true) {
                this.queryData();
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    };

    showDeleteModal = (item) => {
        Modal.confirm({
            title: '删除选项',
            content: '确定删除该选项吗？',
            okText: '确认',
            cancelText: '取消',
            onOk: () => this.deleteItem(item.id),
        });
    };

    queryDataFirst = () => {
        this.setState({
            pageNum: 1
        }, () => {
            this.queryData();
        });
    }

    queryData = () => {
        this.setState({ loading: true });
        const { dispatch, form } = this.props;

        let search = this.state.queryParam;
        form.validateFields((err, fieldsValue) => {
            if (err) return;
            search = {
                ...fieldsValue,
            };
            this.setState({ queryParam: search });
        });

        let params = {
            search: search,
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
                type: 'product/grid',
                payload: {
                    resolve,
                    params,
                },
            });
        }).then(res => {
            this.setState({ loading: false });
            if (res.success === true) {
                this.setState({
                    pageData: res.data
                });
            } else {
                notification.error({
                    message: res.message,
                });
            }
        });
    }

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
        var firstPage = this.state.predicate && sorter.field != this.state.predicate;
        this.setState({
            pageNum: pagination.current,
            pageSize: pagination.pageSize
        }, () => {
            if (sorter.field) {
                this.setState({
                    predicate: sorter.field,
                    reverse: sorter.order == 'descend'
                }, () => {
                    if (firstPage)
                        this.queryDataFirst();
                    else
                        this.queryData();
                });
            } else {
                if (firstPage)
                    this.queryDataFirst();
                else
                    this.queryData();
            }
        });
    };

    handleAdd = () => {
        router.push('./info');
    }

    handleEdit = (id) => {
        router.push({
            pathname: './info',
            query: {
                id: id,
            },
        });
    }

    renderForm() {
        const { form: { getFieldDecorator }, } = this.props;
        return (
            <Form onSubmit={this.handleSearch} layout="inline">
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col md={8} sm={24}>
                        <FormItem label="商品名称">
                            {getFieldDecorator('name')(<Input
                                allowClear
                                placeholder="商品名称" />)}
                        </FormItem>
                    </Col>
                    <Col md={8} sm={24}>
                        <FormItem label="是否发布">
                            {getFieldDecorator('isPublished')(
                                <Select
                                    allowClear
                                    placeholder="是否发布">
                                    <Option value={'false'}>否</Option>
                                    <Option value={'true'}>是</Option>
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col md={8} sm={24}>
                        <FormItem label="有选项">
                            {getFieldDecorator('hasOptions')(
                                <Select
                                    allowClear
                                    placeholder="有选项">
                                    <Option value={'false'}>否</Option>
                                    <Option value={'true'}>是</Option>
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                {this.state.expandForm ? this.getAdvancedFields() : null}
                <Row>
                    <Col span={12} >
                        <span className={styles.submitButtons}>
                            <Button type="primary" htmlType="submit" icon="search">查询</Button>
                            <Button style={{ marginLeft: 8 }} onClick={this.handleFormReset} icon="undo">重置</Button>
                            <a style={{ marginLeft: 8 }} onClick={this.toggleForm}>
                                {this.state.expandForm ? '收起' : '展开'}
                                <Icon type={this.state.expandForm ? 'up' : 'down'} />
                            </a>
                        </span>
                    </Col>
                    {/* <Col span={12} style={{ textAlign: 'right' }}>
                        <span >
                        </span>
                    </Col> */}
                </Row>

            </Form>
        );
    }

    getAdvancedFields() {
        const { form: { getFieldDecorator }, } = this.props;
        return (
            <Fragment>
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col md={8} sm={24}>
                        <FormItem label="单独可见">
                            {getFieldDecorator('isVisibleIndividually')(
                                <Select
                                    allowClear
                                    placeholder="单独可见">
                                    <Option value={'false'}>否</Option>
                                    <Option value={'true'}>是</Option>
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col md={8} sm={24}>
                        <FormItem label="允许订购">
                            {getFieldDecorator('isAllowToOrder')(
                                <Select
                                    allowClear
                                    placeholder="允许订购">
                                    <Option value={'false'}>否</Option>
                                    <Option value={'true'}>是</Option>
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col md={8} sm={24}>
                        <FormItem label="SKU">
                            {getFieldDecorator('sku')(<Input
                                allowClear
                                placeholder="SKU" />)}
                        </FormItem>
                    </Col>
                </Row>
                <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
                    <Col md={16} sm={24}>
                        <FormItem
                            label={<span>商品分类</span>}>
                            {getFieldDecorator('categoryIds')
                                (<Select
                                    mode="multiple"
                                    placeholder="请选择商品分类"
                                    allowClear={true}>
                                    {
                                        this.state.categories.map(c => {
                                            return <Option value={c.id} key={c.id}>{c.name}</Option>;
                                        })
                                    }
                                </Select>)}
                        </FormItem>
                    </Col>
                    <Col md={8} sm={24}>
                        <FormItem >
                            {getFieldDecorator('includeSubCategories')(<Checkbox>自动搜索子类别</Checkbox>)}
                        </FormItem>
                    </Col>
                </Row>
            </Fragment>
        );
    }

    handleFormReset = () => {
        const { form, dispatch } = this.props;
        form.resetFields();
        this.setState({
            formValues: {},
        });
    };

    toggleForm = () => {
        const { expandForm } = this.state;
        this.setState({
            expandForm: !expandForm,
        });
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
            }
        };
        const action = (
            <Fragment>
                <Button onClick={this.handleAdd}
                    type="primary"
                    icon="plus">添加</Button>
            </Fragment>
        );
        return (
            <PageHeaderWrapper title="商品" action={
                <Button
                    onClick={this.handleAdd}
                    type="primary"
                    icon="plus"
                >添加</Button>
            }>
                <div>
                    <Card bordered={false}>
                        <div className={styles.tableListForm}>{this.renderForm()}</div>
                        {/* <div style={{ marginBottom: '20px' }} >
                            {action}
                        </div> */}
                        <StandardTable
                            pagination={pagination}
                            loading={this.state.loading}
                            data={this.state.pageData}
                            rowKey={record => record.id}
                            columns={this.columns}
                            bordered
                            onChange={this.handleStandardTableChange}
                            scroll={{ x: 1600 }}
                        />
                    </Card>
                </div>
            </PageHeaderWrapper>
        );
    }
}

export default ProductList;