import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
    Row, Col, Card, Form, Input, Button, Table, notification,
    Popconfirm, Switch, Tag, Select, Divider, Menu, Dropdown, Icon
} from 'antd';

import StandardTable from '@/components/StandardTable';
import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import { routerRedux } from 'dva/router';
import router from 'umi/router';
import Link from 'umi/link';

const FormItem = Form.Item;
const WidgetWithId = [
    { key: 1, value: "Category Widget", desc: "分类部件", createUrl: "widget-category-create" },
    { key: 2, value: "Product Widget", desc: "商品部件", createUrl: "widget-product-create" },
    { key: 3, value: "Simple Product Widget", desc: "简单商品部件", createUrl: "widget-simple-product-create" },
    { key: 4, value: "Html Widget", desc: "Html部件", createUrl: "widget-html-create" },
    { key: 5, value: "Carousel Widget", desc: "轮播部件", createUrl: "widget-carousel-create" },
    // { key: 6, value: "Space Bar Widget", desc: "空部件", createUrl: "widget-spacebar-create" },
    { key: 7, value: "Recently Viewed Widget", desc: "最近浏览部件", createUrl: "widget-recently-viewed-create" }
];
const WidgetZoneWithId = [
    { key: 1, value: "Home Featured", desc: "首页头部" },
    { key: 2, value: "Home Main Content", desc: "首页主体" },
    { key: 3, value: "Home After Main Content", desc: "首页主体之后" }
];

@connect()
@Form.create()
class WidgetList extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            loading: false,
            data: []
        };
    }

    columns = [
        {
            title: '操作',
            key: 'operation',
            fixed: 'left',
            align: 'center',
            width: 100,
            render: (text, record) => (
                <Fragment>
                    <Button.Group>
                        <Button icon="edit" size="small" onClick={() => this.handleEdit(text, record)}></Button>
                        <Popconfirm title="确定要删除吗？" onConfirm={() => this.handleDelete(text, record)}>
                            <Button icon="delete" type="danger" size="small"></Button>
                        </Popconfirm>
                    </Button.Group>
                </Fragment>
            )
        },
        // {
        //   title: 'ID',
        //   dataIndex: 'id',
        //   fixed: 'left',
        //   sorter: true,
        //   defaultSortOrder: 'descend',
        //   width: 120,
        // },
        {
            title: '名称',
            dataIndex: 'name',
        },
        {
            title: '类型',
            dataIndex: 'widgetType',
            width: 160,
        },
        {
            title: '区域',
            dataIndex: 'widgetZone',
            width: 160,
        },
        {
            title: '发布开始时间',
            dataIndex: 'publishStart',
            width: 140,
            // render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },
        {
            title: '发布结束时间',
            dataIndex: 'publishEnd',
            width: 140,
            // render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        },
        {
            title: '显示顺序',
            dataIndex: 'displayOrder',
            width: 100,
        }
    ];

    handleSearch = () => {
        this.setState({ loading: true });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'widget/list',
                payload: {
                    resolve,
                    params: {},
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
    };

    handleDelete = (text, record) => {
        this.setState({ loading: true });
        const { dispatch } = this.props;
        new Promise(resolve => {
            dispatch({
                type: 'widget/delete',
                payload: {
                    resolve,
                    params: { id: record.id },
                },
            });
        }).then(res => {
            this.setState({ loading: true });
            if (res.success === true) {
                this.handleSearch();
            } else {
                notification.error({ message: res.message, });
            }
        });
    };

    handleMenuClick = (e) => {
        let widget = WidgetWithId.find(c => c.key == e.key);
        if (widget) {
            router.push(widget.createUrl);
        }
    }

    handleEdit = (text, record) => {
        router.push(record.editUrl + '/' + record.id);
    }

    componentDidMount() {
        this.handleSearch();
    }

    render() {
        const menu = (
            <Menu onClick={this.handleMenuClick}>
                {
                    WidgetWithId.map(c => <Menu.Item key={c.key}>{c.desc}</Menu.Item>)
                }
            </Menu>
        );
        return (
            <PageHeaderWrapper title="首页小部件配置" action={<Dropdown overlay={menu}>
                <Button
                    type="primary"
                    icon="plus">
                    添加<Icon type="down" />
                </Button>
            </Dropdown>} >
                <Card bordered={false}>
                    <div>
                        <Table
                            bordered={false}
                            rowKey={record => record.id}
                            pagination={false}
                            dataSource={this.state.data}
                            columns={this.columns}
                            loading={this.state.loading}
                        // scroll={{ x: 1100 }}
                        />
                    </div>
                </Card>
            </PageHeaderWrapper>
        );
    }
}

export default WidgetList;
