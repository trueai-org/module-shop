import React, { PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import moment from 'moment';
import {
  Row, Col, Card, Form, Input, Button, Table, notification,
  Popconfirm, Switch, Tag, Select, Divider
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';
import BrandComponent from './BrandComponent';

const FormItem = Form.Item;

@connect()
@Form.create()
class TableList extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      visible: false,
      loading: false,
      keyword: '',
      pageNum: 1,
      pageSize: 5,
      predicate: 'id',
      reverse: true,
      data: {
        list: [],
        pagination: {}
      },

      submitting: false,
      brand: {}
    };

    this.handleChangeKeyword = this.handleChangeKeyword.bind(this);
  }

  columns = [
    {
      title: '操作',
      key: 'operation',
      // fixed: 'left',
      align: 'center',
      width: 120,
      render: (text, record) => (
        <Fragment>
          <Button.Group>
            <Button icon="edit" size="small" onClick={() => this.handleUpdateModalVisible(text, record)}></Button>
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
    //   // fixed: 'left',
    //   sorter: true,
    //   defaultSortOrder: 'descend',
    //   width: 120,
    // },
    {
      title: '名称',
      dataIndex: 'name',
      sorter: true,
    },
    {
      title: '是否发布',
      dataIndex: 'isPublished',
      sorter: true,
      width: 120,
      render: (val) => <Switch checked={val} disabled />
    },
    {
      title: '创建时间',
      dataIndex: 'createdOn',
      sorter: true,
      width: 120,
      render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
    },
    // {
    //   title: '更新时间',
    //   dataIndex: 'updatedOn',
    //   sorter: true,
    //   width: 120,
    //   render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
    // },
  ];

  handleUpdateModalVisible = (text, record) => {
    if (record.id) {
      this.setState({
        visible: true,
        brand: record
      });
    }
  };

  handleChangeKeyword(event) {
    this.setState({
      keyword: event.target.value,
    });
  }

  showModal = () => {
    this.setState({
      visible: true,
      brand: {}
    });
  };

  saveFormRef = (formRef) => {
    this.formRef = formRef;
  }

  handleCreate = () => {
    const { dispatch } = this.props;
    const form = this.formRef.props.form;
    form.validateFields((err, values) => {
      if (err) {
        return;
      }
      var params = {
        ...values
      };

      let bt = 'brand/addBrand';
      if (params.id) {
        bt = 'brand/editBrand';
      }

      if (this.state.submitting === true)
        return;

      // console.log(params);

      this.setState({ submitting: true });
      new Promise(resolve => {
        dispatch({
          type: bt,
          payload: {
            resolve,
            params
          },
        });
      }).then(res => {
        this.setState({ submitting: false });
        if (res.success === true) {
          form.resetFields();
          this.setState({ visible: false });
          this.handleSearchFirst();
        } else {
          notification.error({
            message: res.message,
          });
        }
      });
    });
  }

  handleCancel = e => {
    this.setState({
      visible: false,
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
            this.handleSearchFirst();
          else
            this.handleSearch();
        });
      } else {
        if (firstPage)
          this.handleSearchFirst();
        else
          this.handleSearch();
      }
    });
  };

  handleDelete = (text, record) => {
    this.setState({
      loading: true,
    });
    const { dispatch } = this.props;
    const params = {
      id: record.id,
    };
    new Promise(resolve => {
      dispatch({
        type: 'brand/deleteBrand',
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
        this.handleSearch();
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleSearch = () => {
    this.setState({
      loading: true,
    });
    const { dispatch } = this.props;
    const params =
    {
      pagination: {
        current: this.state.pageNum,
        pageSize: this.state.pageSize
      },
      search: {
        predicateObject: {
          name: this.state.keyword
        }
      },
      sort: {
        predicate: this.state.predicate,
        reverse: this.state.reverse
      }
    };

    new Promise(resolve => {
      dispatch({
        type: 'brand/queryBrand',
        payload: {
          resolve,
          params,
        },
      });
    }).then(res => {
      if (res.success === true) {
        this.setState({
          loading: false,
          data: res.data
        });
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleSearchFirst = () => {
    this.setState({
      pageNum: 1
    }, () => {
      this.handleSearch();
    });
  }

  componentDidMount() {
    this.handleSearchFirst();
  }

  renderSimpleForm() {
    return (
      <Form layout="inline" style={{ marginBottom: '20px' }}>
        <Row gutter={{ md: 8, lg: 24, xl: 48 }}>
          <Col md={24} sm={24}>
            <FormItem>
              <Input
                placeholder="名称"
                value={this.state.keyword}
                onChange={this.handleChangeKeyword} />
            </FormItem>
            <span>
              <Button
                onClick={this.handleSearch}
                style={{ marginTop: '3px' }}
                type="primary"
                icon="search">
                查询</Button>
            </span>
            {/* <span>
              <Button
                style={{ marginTop: '3px', marginLeft: '20px' }}
                onClick={this.showModal}
                type="primary"
                icon="plus">
                新增</Button>
            </span> */}
          </Col>
        </Row>
      </Form>
    );
  }

  render() {
    const { pageNum, pageSize } = this.state;
    const pagination = {
      showQuickJumper: true,
      showSizeChanger: true,
      pageSizeOptions: ['5', '10', '50', '100'],
      defaultPageSize: pageSize,
      defaultCurrent: pageNum,
      current: pageNum,
      pageSize: pageSize,
      total: this.state.data.pagination.total || 0,
      showTotal: (total, range) => {
        return `${range[0]}-${range[1]} 条 , 共 ${total} 条`;
      }
    };

    return (
      <PageHeaderWrapper title="商品品牌" action={
        <Button
          // style={{ marginTop: '3px', marginLeft: '20px' }}
          onClick={this.showModal}
          type="primary"
          icon="plus">
          添加</Button>
      }>
        <Card bordered={false}>
          <div className="">
            <div className="">{this.renderSimpleForm()}</div>
            <StandardTable
              pagination={pagination}
              loading={this.state.loading}
              data={this.state.data}
              rowKey={record => record.id}
              columns={this.columns}
              bordered
              onChange={this.handleStandardTableChange}
            // scroll={{ x: 800 }}
            />
          </div>
        </Card>
        <BrandComponent
          visible={this.state.visible}
          brand={this.state.brand}
          wrappedComponentRef={this.saveFormRef}
          showModal={this.showModal}
          handleCancel={this.handleCancel}
          onCreate={this.handleCreate} />
      </PageHeaderWrapper>
    );
  }
}

export default TableList;
