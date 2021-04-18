import React from 'react';
import { Row, Col, Input, Button, Modal, Form, Avatar } from 'antd';
import moment from 'moment';
import StandardTable from '@/components/StandardTable';
import { formatBool } from '@/utils/utils';

@Form.create()
class ProductCommponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: '',
            selectedRowKeys: []
        };
    }
    columns = [
        {
            title: 'ID',
            dataIndex: 'id',
            width: 100,
            // sorter: true,
            // defaultSortOrder: 'descend',
        },
        {
            title: '商品名称',
            dataIndex: 'name',
        },
        {
            title: '价格',
            dataIndex: 'price',
            width: 120,
        },
        {
            title: '是否发布',
            dataIndex: 'isPublished',
            width: 100,
            render: (val) => formatBool(val)
        },
        {
            title: '图片',
            dataIndex: 'mediaUrl',
            align: 'center',
            width: 64,
            // fixed: 'right',
            render: (text, record) => <Avatar shape="square" size={32} src={record.mediaUrl} />
        },
        // {
        //     title: '创建时间',
        //     dataIndex: 'createdOn',
        //     width: 120,
        //     render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
        // }
    ];
    componentDidMount() { }
    renderForm() {
        return (
            <Form layout="inline">
                <Row gutter={{ md: 8, lg: 24, xl: 48 }} style={{ marginBottom: 12 }}>
                    <Col md={12} sm={24}>
                        <Input
                            onChange={(e) => {
                                this.setState({ name: e.target.value });
                            }}
                            allowClear
                            placeholder="商品名称" />
                    </Col>
                    <Col md={12} sm={24}>
                        <Button onClick={() => { this.props.handleSearch(this.state.name); }} htmlType="submit" type="primary" icon="search">查询</Button>
                    </Col>
                </Row>
            </Form>
        )
    }
    onSelectChange = (selectedRowKeys) => {
        // console.log('selectedRowKeys changed: ', selectedRowKeys);
        this.setState({ selectedRowKeys });
    }
    render() {
        const { selectedRowKeys } = this.state;
        const rowSelection = {
            selectedRowKeys,
            onChange: this.onSelectChange,
        };

        return (
            <div>
                <Modal
                    title={`选择商品`}
                    width={680}
                    destroyOnClose
                    visible={this.props.visible}
                    onCancel={this.props.onCancel}
                    onOk={() => { this.props.onOk(this.state.selectedRowKeys) }}
                >
                    <div>{this.renderForm()}</div>
                    <StandardTable
                        rowSelection={rowSelection}
                        pagination={this.props.pagination}
                        loading={this.props.loading}
                        data={this.props.pageData}
                        rowKey={record => record.id}
                        columns={this.columns}
                        onChange={this.props.onChange}
                        bordered={false}
                    />
                </Modal>
            </div>
        );
    }
}

export default ProductCommponent;
