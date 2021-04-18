import React, { Component, PureComponent, Fragment } from 'react';
import { connect } from 'dva';
import { formatMessage, FormattedMessage } from 'umi/locale';
import {
  List,
  Card,
  Input,
  Button,
  Modal,
  Form,
  notification,
  Table,
  Popconfirm,
  Divider,
  Select,
  Tag,
  Icon,
  Menu,
  Dropdown,
  Checkbox,
  Switch,
  Tabs,
  InputNumber,
  Upload,
  DatePicker,
  Avatar,
  Spin,
  Radio,
  Tooltip,
} from 'antd';

import PageHeaderWrapper from '@/components/PageHeaderWrapper';
import StandardTable from '@/components/StandardTable';
import router from 'umi/router';
import Link from 'umi/link';
import moment from 'moment';

import { SketchPicker } from 'react-color';

// editor
// import { EditorState, convertToRaw } from 'draft-js';
// import { Editor } from 'react-draft-wysiwyg';
// import draftToHtml from 'draftjs-to-html';
// import htmlToDraft from 'html-to-draftjs';

// editor2
import 'braft-editor/dist/index.css';
import BraftEditor from 'braft-editor';

import styles from './Info.less';

import CopyCommponent from './CopyCommponent';

const RangePicker = DatePicker.RangePicker;
const FormItem = Form.Item;
const { Option, OptGroup } = Select;
const TabPane = Tabs.TabPane;
const { TextArea } = Input;

@connect()
@Form.create()
class ProductInfo extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      id: props.location.query.id, //商品id
      current: {}, //商品数据
      loading: false, //商品数据加载中

      submitting: false, //数据保存中

      uploadLoading: false,
      uploadMainLoading: false,
      previewVisible: false,
      previewImage: '',
      fileList: [],

      categoryLoading: false, //类别加载中
      categories: [],

      brandLoading: false, //品牌加载中
      brands: [],

      optionLoading: false, //选项加载中
      options: [],
      optionCurrent: undefined,

      templateLoading: false, //属性模板加载中
      templates: [],
      templateCurrent: undefined,
      //应用商品属性模板
      applyLoading: false,

      attributeLoading: false, //属性加载中
      attributes: [],
      attributeCurrent: undefined,

      //商品属性列表
      productAttributeLoading: false,
      productAttributeData: [],
      //属性值
      attributeData: [],

      //商品选项列表
      productOptionDataLoading: false,
      productOptionData: [],
      //选项值
      optionData: [],

      //商品规格列表
      productSkuLoading: false,
      productSku: [],

      //选项配置
      visibleOptionSetting: false,
      currentColor: '',

      //添加选项组合
      visibleOptionAdd: false,
      addOptionCombination: [],

      optionSettingCurrent: {},

      //辅助商品
      visibleCopy: false,
      copyProduct: {},
      copySubmitting: false,

      //发布
      currentIsPublished: undefined,
      currentPublishType: undefined,
      //库存跟踪
      currentStockTrackingIsEnabled: undefined,

      warehouses: [],
      warehousesLoading: false,

      //库存历史
      historyLoading: false,
      pageNum: 1,
      pageSize: 5,
      predicate: 'Id',
      reverse: true,
      historyData: {
        list: [],
        pagination: {},
      },

      //航运
      isShipEnabled: false,

      //单位
      units: [],
      unitsLoading: false,

      //产品库存
      productStocks: [],
      productStocksLoading: false,

      //sku库存
      visibleSkuStocks: false,
      skuStockCurrent: {},
    };
  }

  columnsAttribute = [
    {
      title: '属性名称',
      dataIndex: 'name',
      width: 150,
    },
    {
      title: '属性值',
      dataIndex: 'value',
      render: (text, record) => (
        <Fragment>
          <Select
            // loading={record.loading}
            mode="tags"
            placeholder="Please select"
            allowClear={true}
            // labelInValue
            onChange={value => {
              if (value) {
                var vs = [];
                value.forEach(c => {
                  // vs.push(c.label);
                  vs.push({ id: 0, value: c });
                });
                let obj = this.state.productAttributeData.find(c => c.id == record.id);
                if (obj) {
                  obj.values = vs;
                }
                // console.log(this.state.productAttributeData);
              }
            }}
            defaultValue={record.values.map(x => x.value)}
          >
            {this.state.attributeData.map(item => {
              let os = [];
              if (item.id == record.id) {
                item.list.forEach(c => {
                  os.push(<Option key={c.value}>{c.value}</Option>);
                });
              }
              return os;
            })}
          </Select>
        </Fragment>
      ),
    },
    {
      title: '操作',
      key: 'operation',
      align: 'center',
      width: 100,
      render: (text, record) => (
        <Fragment>
          <Button
            onClick={() => this.handleRemoveProductAttribute(record)}
            icon="close"
            type="danger"
            size="small"
          />
        </Fragment>
      ),
    },
  ];

  columnsOption = [
    {
      title: '选项名称',
      dataIndex: 'name',
      width: 150,
    },
    {
      title: '选项值',
      dataIndex: 'value',
      render: (text, record) => (
        <Fragment>
          <Select
            // loading={record.loading}
            mode="tags"
            placeholder="Please select"
            allowClear={true}
            // labelInValue
            onChange={value => {
              if (value) {
                let obj = this.state.productOptionData.find(c => c.id == record.id);
                if (obj) {
                  let ops = [];
                  value.forEach(x => {
                    var v = obj.values.find(c => c.value == x);
                    if (v) {
                      v.value = x;
                      ops.push(v);
                    } else {
                      let p = {
                        id: 0,
                        value: x,
                        display: '',
                        displayOrder: 0,
                        mediaUrl: '',
                        mediaId: '',
                      };
                      let opValues = this.state.optionData.find(c => c.id == record.id);
                      if (opValues && opValues.values.length > 0) {
                        let ov = opValues.values.find(c => c.value == x);
                        if (ov) {
                          p.id = ov.id;
                          p.display = ov.display;
                        }
                      }
                      ops.push(p);
                    }
                  });
                  obj.values = ops;
                }
              }
            }}
            defaultValue={record.values.map(x => {
              // return { key: x.value }
              return x.value;
            })}
          >
            {this.state.optionData.map(item => {
              let os = [];
              if (item.id == record.id) {
                item.values.forEach(c => {
                  os.push(<Option key={c.value}>{c.value}</Option>);
                });
              }
              return os;
            })}
          </Select>
        </Fragment>
      ),
    },
    {
      title: '操作',
      key: 'operation',
      align: 'center',
      width: 100,
      render: (text, record) => (
        <Fragment>
          <Button.Group>
            <Button
              onClick={() => this.showOptionSettingModal(record)}
              icon="setting"
              type=""
              size="small"
            />
            <Button
              onClick={() => this.handleRemoveProductOption(record)}
              icon="close"
              type="danger"
              size="small"
            />
          </Button.Group>
        </Fragment>
      ),
    },
  ];

  columnsOptionSetting = [
    {
      title: '选项值',
      dataIndex: 'value',
    },
    {
      title: '显示',
      dataIndex: 'display',
      width: 120,
      render: (text, record) => (
        <Fragment>
          <Input
            onChange={e => {
              let obj = this.state.optionSettingCurrent.values.find(c => c.value == record.value);
              if (obj) {
                obj.display = e.target.value;
              }
            }}
            defaultValue={text}
            style={
              this.state.optionSettingCurrent.displayType == 1
                ? {
                    backgroundColor: record.display || '',
                  }
                : {}
            }
            // value={text}
            onClick={() => {
              this.state.optionSettingCurrent.displayType == 1
                ? this.setState({ currentColor: record.display || '' }, () => {
                    Modal.info({
                      title: '选择颜色',
                      content: (
                        <SketchPicker
                          color={this.state.currentColor || ''}
                          onChange={color => {
                            let olds = this.state.optionSettingCurrent.values;
                            let obj = olds.find(c => c.value == record.value);
                            if (obj) {
                              let index = olds.indexOf(obj);
                              let list = olds.slice();
                              list.splice(index, 1);
                              olds = list;

                              obj.display = color.hex;
                              olds.push(obj);
                            }
                            this.setState({
                              'optionSettingCurrent.values': olds,
                            });
                            this.setState({ currentColor: color.hex });
                          }}
                        />
                      ),
                      okText: '关闭',
                    });
                  })
                : {};
            }}
          />
        </Fragment>
      ),
    },
    {
      title: '显示顺序',
      dataIndex: 'displayOrder',
      width: 120,
      render: (text, record) => (
        <InputNumber
          width={110}
          onChange={v => {
            let obj = this.state.optionSettingCurrent.values.find(c => c.value == record.value);
            if (obj) {
              obj.displayOrder = v;
            }
          }}
          defaultValue={text}
        />
      ),
    },
    {
      title: '默认',
      dataIndex: 'isDefault',
      width: 80,
      render: (val, record) => (
        <Switch
          onChange={e => {
            let obj = this.state.optionSettingCurrent.values.find(c => c.value == record.value);
            if (obj) {
              obj.isDefault = e;
            }
          }}
          defaultChecked={val}
        />
      ),
    },
    {
      title: '图片',
      dataIndex: 'mediaId',
      width: 80,
      // align: 'center',
      render: (text, record) => (
        <Fragment>
          <Avatar
            onClick={() => {
              Modal.info({
                title: '选择图片',
                content: (
                  <Radio.Group
                    defaultValue={record.mediaId || ''}
                    onChange={e => {
                      let olds = this.state.optionSettingCurrent.values;
                      let obj = olds.find(c => c.value == record.value);
                      if (obj) {
                        let index = olds.indexOf(obj);
                        let list = olds.slice();
                        list.splice(index, 1);
                        olds = list;

                        obj.mediaId = '';
                        obj.mediaUrl = '';
                        if (e.target.value) {
                          let first = this.state.fileList.find(c => c.mediaId == e.target.value);
                          if (first) {
                            obj.mediaId = first.mediaId;
                            obj.mediaUrl = first.url;
                          }
                        }
                        olds.push(obj);
                        this.setState({
                          'optionSettingCurrent.values': olds,
                        });
                      }
                    }}
                  >
                    <Radio
                      style={{
                        width: 80,
                      }}
                      value={''}
                    >
                      无
                    </Radio>
                    {this.state.fileList.map(x => {
                      return (
                        <Radio
                          style={{
                            width: 80,
                          }}
                          key={x.mediaId}
                          value={x.mediaId}
                        >
                          <Avatar shape="square" size={48} src={x.url} />
                        </Radio>
                      );
                    })}
                  </Radio.Group>
                ),
                okText: '关闭',
              });
            }}
            shape="square"
            size={32}
            src={record.mediaUrl}
          />
        </Fragment>
      ),
    },
  ];

  columnsSku = [
    {
      title: '名称',
      dataIndex: 'name',
      // width: 150,
      render: (text, record) => (
        <Fragment>
          <Input
            onChange={e => {
              // let obj = this.state.productSku.find(c => c.id == record.id);
              // if (obj) {
              //     obj.name = e.target.value;
              // }

              let index = this.state.productSku.indexOf(record);
              if (index >= 0) {
                let list = this.state.productSku.slice();
                list.splice(index, 1);

                record.name = e.target.value;
                list.splice(index, 0, record);
                this.setState({ productSku: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: 'SKU',
      dataIndex: 'sku',
      width: 150,
      render: (text, record) => (
        <Fragment>
          <Input
            onChange={e => {
              // let obj = this.state.productSku.find(c => c.id == record.id);
              // if (obj) {
              //     obj.sku = e.target.value;
              // }

              let index = this.state.productSku.indexOf(record);
              if (index >= 0) {
                let list = this.state.productSku.slice();
                list.splice(index, 1);

                record.sku = e.target.value;
                list.splice(index, 0, record);
                this.setState({ productSku: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: 'GTIN',
      dataIndex: 'gtin',
      width: 150,
      render: (text, record) => (
        <Fragment>
          <Input
            onChange={e => {
              // let obj = this.state.productSku.find(c => c.id == record.id);
              // if (obj) {
              //     obj.gtin = e.target.value;
              // }
              let index = this.state.productSku.indexOf(record);
              if (index >= 0) {
                let list = this.state.productSku.slice();
                list.splice(index, 1);

                record.gtin = e.target.value;
                list.splice(index, 0, record);
                this.setState({ productSku: list });
              }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '价格',
      dataIndex: 'price',
      width: 100,
      render: (value, record) => (
        <Fragment>
          <InputNumber
            onChange={e => {
              // let obj = this.state.productSku.find(c => c.id == record.id);
              // if (obj) {
              //     obj.price = e;
              // }
              let index = this.state.productSku.indexOf(record);
              if (index >= 0) {
                let list = this.state.productSku.slice();
                list.splice(index, 1);

                record.price = e;
                list.splice(index, 0, record);
                this.setState({ productSku: list });
              }
            }}
            defaultValue={value}
          />
        </Fragment>
      ),
    },
    {
      title: '原价',
      dataIndex: 'oldPrice',
      width: 100,
      render: (value, record) => (
        <Fragment>
          <InputNumber
            onChange={e => {
              // let obj = this.state.productSku.find(c => c.id == record.id);
              // if (obj) {
              //     obj.oldPrice = e;
              // }
              let index = this.state.productSku.indexOf(record);
              if (index >= 0) {
                let list = this.state.productSku.slice();
                list.splice(index, 1);

                record.oldPrice = e;
                list.splice(index, 0, record);
                this.setState({ productSku: list });
              }
            }}
            defaultValue={value}
          />
        </Fragment>
      ),
    },
    {
      title: '库存',
      dataIndex: 'stockQuantity',
      width: 100,
      // render: (value, record) => (
      //     <Fragment>
      //         <InputNumber
      //             min={0}
      //             precision={0}
      //             onChange={(e) => {
      //                 // let obj = this.state.productSku.find(c => c.name == record.name);
      //                 // if (obj) {
      //                 //     obj.stockQuantity = e;
      //                 // }

      //                 let index = this.state.productSku.indexOf(record);
      //                 if (index >= 0) {
      //                     let list = this.state.productSku.slice();
      //                     list.splice(index, 1);

      //                     record.stockQuantity = e;
      //                     list.splice(index, 0, record);
      //                     this.setState({ productSku: list });
      //                 }
      //             }}
      //             defaultValue={value}></InputNumber>
      //     </Fragment>
      // )
    },
    {
      title: '图片',
      dataIndex: 'mediaId',
      align: 'center',
      width: 64,
      fixed: 'right',
      render: (text, record) => (
        <Fragment>
          <Avatar
            onClick={() => {
              Modal.info({
                title: '选择图片',
                content: (
                  <Radio.Group
                    defaultValue={record.mediaId || ''}
                    onChange={e => {
                      let index = this.state.productSku.indexOf(record);
                      let list = this.state.productSku.slice();
                      list.splice(index, 1);
                      record.mediaId = '';
                      record.mediaUrl = '';
                      if (e.target.value) {
                        let first = this.state.fileList.find(c => c.mediaId == e.target.value);
                        if (first) {
                          record.mediaId = first.mediaId;
                          record.mediaUrl = first.url;
                        }
                      }
                      // list.push(record);
                      list.splice(index, 0, record);
                      this.setState({ productSku: list });
                    }}
                  >
                    <Radio
                      style={{
                        width: 80,
                      }}
                      value={''}
                    >
                      无
                    </Radio>
                    {this.state.fileList.map(x => {
                      return (
                        <Radio
                          style={{
                            width: 80,
                          }}
                          key={x.mediaId}
                          value={x.mediaId}
                        >
                          <Avatar shape="square" size={48} src={x.url} />
                        </Radio>
                      );
                    })}
                  </Radio.Group>
                ),
                okText: '关闭',
              });
            }}
            shape="square"
            size={32}
            src={record.mediaUrl}
          />
        </Fragment>
      ),
    },
    {
      title: '操作',
      key: 'operation',
      align: 'center',
      width: 100,
      fixed: 'right',
      render: (text, record) => (
        <Fragment>
          <Button.Group>
            <Button
              onClick={() => this.showSkuStocksModal(record)}
              icon="setting"
              type=""
              size="small"
            />
            <Button
              onClick={() => this.handleRemoveSku(record)}
              icon="close"
              type="danger"
              size="small"
            />
          </Button.Group>
        </Fragment>
      ),
    },
  ];

  columnsHistory = [
    {
      title: 'Id',
      dataIndex: 'id',
      width: 100,
      defaultSortOrder: 'descend',
      sorter: true,
    },
    {
      title: '仓库',
      dataIndex: 'warehouseName',
    },
    {
      title: '调整数量',
      dataIndex: 'adjustedQuantity',
      width: 120,
      sorter: true,
      render: val => (
        <span
          style={{
            color: val > 0 ? '#52c41a' : '#f5222d',
          }}
        >
          {val > 0 ? '+' : ''}
          {val}
        </span>
      ),
    },
    {
      title: '库存数量',
      dataIndex: 'stockQuantity',
      width: 120,
      sorter: true,
    },
    {
      title: '备注',
      dataIndex: 'note',
      sorter: true,
    },
    {
      title: '创建时间',
      dataIndex: 'createdOn',
      width: 120,
      sorter: true,
      // defaultSortOrder: 'descend',
      render: val => <span>{moment(val).format('YYYY-MM-DD')}</span>,
    },
  ];

  columnsProductStock = [
    {
      title: '仓库',
      dataIndex: 'name',
    },
    {
      title: '库存',
      dataIndex: 'quantity',
      width: 100,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            precision={0}
            onChange={e => {
              record.quantity = e;
              // if (record.type == 'sku') {
              //     let index = this.state.productSku.indexOf(this.state.skuStockCurrent);
              //     if (index >= 0) {
              //         let xxx = this.state.productSku[index];

              //         let stocks = this.state.skuStockCurrent.stocks || [];
              //         let itemIndex = stocks.indexOf(record);
              //         if (itemIndex >= 0) {
              //             let itemList = stocks.slice();
              //             itemList.splice(itemIndex, 1);
              //             record.quantity = e;
              //             itemList.splice(itemIndex, 0, record);
              //             xxx.stocks = itemList;
              //         }

              //         let list = this.state.productSku.slice();
              //         list.splice(index, 1);
              //         list.splice(index, 0, xxx);
              //         this.setState({
              //             productSku: list,
              //             skuStockCurrent: xxx
              //         });
              //     }
              // } else {
              //     let index = this.state.productStocks.indexOf(record);
              //     if (index >= 0) {
              //         let list = this.state.productStocks.slice();
              //         list.splice(index, 1);
              //         record.quantity = e;
              //         list.splice(index, 0, record);
              //         this.setState({ productStocks: list });
              //     }
              // }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '顺序',
      dataIndex: 'displayOrder',
      width: 100,
      render: (text, record) => (
        <Fragment>
          <InputNumber
            min={0}
            precision={0}
            onChange={e => {
              record.displayOrder = e;
              // if (record.type == 'sku') {
              //     let index = this.state.productSku.indexOf(this.state.skuStockCurrent);
              //     if (index >= 0) {
              //         let xxx = this.state.productSku[index];

              //         let stocks = this.state.skuStockCurrent.stocks || [];
              //         let itemIndex = stocks.indexOf(record);
              //         if (itemIndex >= 0) {
              //             let itemList = stocks.slice();
              //             itemList.splice(itemIndex, 1);
              //             record.displayOrder = e;
              //             itemList.splice(itemIndex, 0, record);
              //             xxx.stocks = itemList;
              //         }

              //         let list = this.state.productSku.slice();
              //         list.splice(index, 1);
              //         list.splice(index, 0, xxx);
              //         this.setState({
              //             productSku: list,
              //             skuStockCurrent: xxx
              //         });
              //     }
              // } else {
              //     let index = this.state.productStocks.indexOf(record);
              //     if (index >= 0) {
              //         let list = this.state.productStocks.slice();
              //         list.splice(index, 1);
              //         record.displayOrder = e;
              //         list.splice(index, 0, record);
              //         this.setState({ productStocks: list });
              //     }
              // }
            }}
            defaultValue={text}
          />
        </Fragment>
      ),
    },
    {
      title: '启用',
      dataIndex: 'isEnabled',
      width: 100,
      render: (val, record) => (
        <Switch
          defaultChecked={val}
          onChange={e => {
            record.isEnabled = e;
            // if (record.type == 'sku') {
            //     let index = this.state.productSku.indexOf(this.state.skuStockCurrent);
            //     if (index >= 0) {
            //         let xxx = this.state.productSku[index];

            //         let stocks = this.state.skuStockCurrent.stocks || [];
            //         let itemIndex = stocks.indexOf(record);
            //         if (itemIndex >= 0) {
            //             let itemList = stocks.slice();
            //             itemList.splice(itemIndex, 1);
            //             record.isEnabled = e;
            //             itemList.splice(itemIndex, 0, record);
            //             xxx.stocks = itemList;
            //         }

            //         let list = this.state.productSku.slice();
            //         list.splice(index, 1);
            //         list.splice(index, 0, xxx);
            //         this.setState({
            //             productSku: list,
            //             skuStockCurrent: xxx
            //         });
            //     }
            // } else {
            //     let index = this.state.productStocks.indexOf(record);
            //     if (index >= 0) {
            //         let list = this.state.productStocks.slice();
            //         list.splice(index, 1);
            //         record.isEnabled = e;
            //         list.splice(index, 0, record);
            //         this.setState({ productStocks: list });
            //     }
            // }
          }}
        />
      ),
    },
  ];

  componentDidMount() {
    this.handleInit();
  }

  handleTabChange = key => {
    if (key == 6) {
      //库存历史
      if (this.state.id) this.handleLoadStockHistory();
    }
  };

  handleSubmit = e => {
    e.preventDefault();
    const { dispatch, form } = this.props;

    form.validateFields((err, values) => {
      if (err) return;

      if (this.state.submitting === true) return;

      this.setState({ submitting: true, loading: true });

      var params = {
        id: this.state.id,
        thumbnailImageUrlId: this.state.current.mediaId || '',
        ...values,
      };

      // 富文本处理
      if (params.description) {
        //draftToHtml(convertToRaw(this.state.editorState.getCurrentContent()))
        params.description = params.description.toHTML(); //draftToHtml(params.description);
      }
      if (params.description) {
        // params.shortDescription = params.shortDescription.toHTML(); //draftToHtml(params.shortDescription);
        params.specification = params.specification.toHTML(); //draftToHtml(params.specification);
      }

      //特价时间处理
      if (params.specialPriceRangePicker && params.specialPriceRangePicker.length == 2) {
        params.specialPriceStart = params.specialPriceRangePicker[0].format('YYYY-MM-DD HH:mm:ss');
        params.specialPriceEnd = params.specialPriceRangePicker[1].format('YYYY-MM-DD HH:mm:ss');
        params.specialPriceRangePicker = {};
      }

      //图片处理
      params.mediaIds = [];
      this.state.fileList.forEach(c => {
        if (c.mediaId) {
          params.mediaIds.push(c.mediaId);
        }
      });

      //商品属性
      params.attributes = [];
      if (this.state.productAttributeData) {
        this.state.productAttributeData.forEach(x => {
          if (x && x.values && x.values.length > 0) {
            let p = { attributeId: x.id, values: x.values.map(p => p.value) };
            params.attributes.push(p);
          }
        });
      }

      //商品选项
      params.options = [];
      this.state.productOptionData.forEach(c => {
        if (c.values && c.values.length > 0) {
          params.options.push({
            id: c.id,
            values: c.values,
          });
        }
      });

      //商品选项组合
      params.variations = [];
      if (this.state.productSku && this.state.productSku.length > 0) {
        params.variations = this.state.productSku;
      }

      //产品库存
      if (params.stockTrackingIsEnabled) {
        params.stocks = this.state.productStocks;
      }

      // console.log(params);
      // return;

      new Promise(resolve => {
        dispatch({
          type: this.state.id ? 'product/edit' : 'product/add',
          payload: {
            resolve,
            params,
          },
        });
      }).then(res => {
        this.setState({ submitting: false, loading: false });
        if (res.success === true) {
          router.push('./list');
        } else {
          notification.error({
            message: res.message,
          });
        }
      });
    });
  };

  showCopyModal = () => {
    this.setState({ visibleCopy: true });
  };

  handleCopyCancel = () => {
    this.setState({ visibleCopy: false });
  };

  saveFormRef = formRef => {
    this.formRef = formRef;
  };

  handleCopySubmit = () => {
    const { dispatch } = this.props;
    const form = this.formRef.props.form;

    form.validateFields((err, values) => {
      if (err) {
        return;
      }
      var params = {
        ...values,
        id: this.state.id,
      };

      if (this.state.copySubmitting === true) return;
      this.setState({ copySubmitting: true });
      new Promise(resolve => {
        dispatch({
          type: 'product/copy',
          payload: {
            resolve,
            params,
          },
        });
      }).then(res => {
        this.setState({ copySubmitting: false });
        if (res.success === true) {
          this.setState({ visibleCopy: false });

          router.push({
            pathname: './info',
            query: {
              id: res.data,
            },
          });

          form.resetFields();
          router.go(0);
        } else {
          notification.error({ message: res.message });
        }
      });
    });
  };

  showDeleteModal = () => {
    Modal.confirm({
      title: '删除商品',
      content: '确定删除该商品吗？',
      okText: '确认',
      cancelText: '取消',
      onOk: () => this.deleteItem(this.state.id),
    });
  };

  deleteItem = id => {
    this.setState({ loading: true });
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
      this.setState({ loading: false });
      if (res.success === true) {
        router.push('./list');
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  showOptionSettingModal = item => {
    this.setState({
      visibleOptionSetting: true,
      optionSettingCurrent: item,
    });
  };

  handleOptionSettingCancel = () => {
    this.setState({
      visibleOptionSetting: false,
      optionSettingCurrent: {},
    });
  };

  showSkuStocksModal = item => {
    this.setState({
      visibleSkuStocks: true,
      skuStockCurrent: item,
    });
  };

  handleSkuStocksCancel = () => {
    this.setState({
      visibleSkuStocks: false,
      skuStockCurrent: {},
    });
  };

  handleAddOptionCombination = () => {
    this.setState({
      visibleOptionAdd: true,
      addOptionCombination: this.state.productOptionData.map(c => {
        return {
          id: c.id,
          value: '',
        };
      }),
    });
  };

  handleAddOptionCombinationOk = () => {
    if (this.state.addOptionCombination.find(c => !c.value)) {
      notification.warning({ message: '请选择选项' });
      return;
    }

    let variation,
      optionCombinations = [];
    this.state.addOptionCombination.forEach((c, index) => {
      let option = this.state.productOptionData.find(x => x.id == c.id);
      if (option) {
        let ov = option.values.find(x => x.value == c.value);
        if (ov) {
          let optionValue = {
            optionName: option.name,
            optionId: option.id,
            value: c.value,
            displayOrder: index,
            mediaId: ov.mediaId || '',
            mediaUrl: ov.mediaUrl || '',
          };
          optionCombinations.push(optionValue);
        }
      }
    });
    if (optionCombinations.length <= 0) {
      return;
    }

    let firstImage = optionCombinations.find(c => c.mediaId && c.mediaId != '');
    variation = {
      id: 0,
      sku: '',
      gtin: this.state.current.gtin || '',
      mediaId: firstImage ? firstImage.mediaId : '',
      mediaUrl: firstImage ? firstImage.mediaUrl : '',
      name:
        (this.state.current.name || '') + ' ' + optionCombinations.map(this.getItemValue).join(' '),
      normalizedName: optionCombinations.map(this.getItemValue).join('-'),
      optionCombinations: optionCombinations,
      price: this.state.current.price || 0,
      oldPrice: this.state.current.oldPrice,
      stockQuantity: this.state.current.stockQuantity || 0,
      stocks: [],
      warehouseIds: [],
    };

    if (!this.state.productSku.find(c => c.name == variation.name)) {
      this.setState({
        productSku: [...this.state.productSku, variation],
      });
    } else {
      notification.warning({ message: '商品组合已存在' });
      return;
    }

    this.setState({ visibleOptionAdd: false });
  };

  handleAddOptionCombinationCancel = () => {
    this.setState({
      visibleOptionAdd: false,
      addOptionCombination: [],
    });
  };

  handleGenerateOptionCombination = () => {
    var optionDatas = this.state.productOptionData;
    if (!optionDatas || optionDatas.length <= 0) return;
    // console.log(optionDatas);

    let maxIndexOption = this.state.productOptionData.length - 1;
    let skus = [];
    this.helper([], 0, maxIndexOption, skus);
    this.setState({ productSku: skus });
  };

  helper = (arr, optionIndex, maxIndexOption, skus) => {
    let j, l, variation, optionCombinations, optionValue;
    for (j = 0, l = this.state.productOptionData[optionIndex].values.length; j < l; j = j + 1) {
      optionCombinations = arr.slice(0);
      optionValue = {
        optionName: this.state.productOptionData[optionIndex].name,
        optionId: this.state.productOptionData[optionIndex].id,
        value: this.state.productOptionData[optionIndex].values[j].value,
        displayOrder: optionIndex,
        mediaId: this.state.productOptionData[optionIndex].values[j].mediaId,
        mediaUrl: this.state.productOptionData[optionIndex].values[j].mediaUrl,
      };
      optionCombinations.push(optionValue);

      if (optionIndex === maxIndexOption) {
        let firstImage = optionCombinations.find(c => c.mediaId && c.mediaId != '');
        variation = {
          id: 0,
          sku: '',
          gtin: this.state.current.gtin || '',
          mediaId: firstImage ? firstImage.mediaId : '',
          mediaUrl: firstImage ? firstImage.mediaUrl : '',
          name:
            (this.state.current.name || '') +
            ' ' +
            optionCombinations.map(this.getItemValue).join(' '),
          normalizedName: optionCombinations.map(this.getItemValue).join('-'),
          optionCombinations: optionCombinations,
          price: this.state.current.price || 0,
          oldPrice: this.state.current.oldPrice,
          stockQuantity: this.state.current.stockQuantity || 0,
          stocks: [],
          warehouseIds: [],
        };
        skus.push(variation);
      } else {
        this.helper(optionCombinations, optionIndex + 1, maxIndexOption, skus);
      }
    }
  };

  getItemValue = item => {
    return item.value;
  };

  handleApplyProductAttrTemplate = () => {
    if (!this.state.templateCurrent || this.state.applyLoading) {
      return;
    }

    this.setState({ applyLoading: true });
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'catalog/templateFirst',
        payload: {
          resolve,
          params: { id: this.state.templateCurrent },
        },
      });
    }).then(res => {
      this.setState({ applyLoading: false });
      if (res.success === true) {
        let list = [];
        let listIds = [];
        list = res.data.attributes;
        listIds = res.data.attributesIds;
        list.forEach(c => {
          this.addProductAttribute(c.id, c.name);
        });
        this.state.productAttributeData.forEach(c => {
          if (listIds.indexOf(c.id) < 0) {
            this.handleRemoveProductAttribute(c);
          }
        });
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleAddProductAttribute = () => {
    if (!this.state.attributeCurrent) {
      return;
    }
    this.addProductAttribute(this.state.attributeCurrent.key, this.state.attributeCurrent.label);
  };

  addProductAttribute = (id, name) => {
    let p = { id, name, values: [], list: [] };
    let any = this.state.productAttributeData.findIndex(c => c.id == p.id) >= 0;
    if (any) return;
    this.setState(
      {
        productAttributeData: [...this.state.productAttributeData, p],
      },
      () => {
        this.queryAttributeData(id, name);
      }
    );
  };

  queryAttributeData = (id, name) => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'catalog/attributeData',
        payload: {
          resolve,
          params: { attributeId: id },
        },
      });
    }).then(res => {
      if (res.success === true) {
        let olds = this.state.attributeData;
        // if (this.state.attributeData.length > 10) {
        //     olds = [];
        // }
        let obj = olds.find(c => c.id == id);
        if (obj) {
          let index = olds.indexOf(obj);
          let list = olds.slice();
          list.splice(index, 1);
          olds = list;
        }
        this.setState({
          attributeData: [
            ...olds,
            {
              id,
              name,
              list: res.data.map(x => {
                return { id: x.id, value: x.value };
              }),
              // list: res.data
            },
          ],
        });
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleAddProductOption = () => {
    if (!this.state.optionCurrent) {
      return;
    }
    this.addProductOption(this.state.optionCurrent.key, this.state.optionCurrent.label);
  };

  addProductOption = (id, name) => {
    let obj = this.state.options.find(c => c.id == id);
    if (obj == undefined) {
      return;
    }
    let p = { id, name, displayType: obj.displayType, values: [] };
    let any = this.state.productOptionData.findIndex(c => c.id == p.id) >= 0;
    if (any) return;
    this.setState(
      {
        productOptionData: [...this.state.productOptionData, p],
      },
      () => {
        this.queryOptionData(id, name);
      }
    );
  };

  queryOptionData = (id, name) => {
    const { dispatch } = this.props;
    new Promise(resolve => {
      dispatch({
        type: 'catalog/optionData',
        payload: {
          resolve,
          params: { optionId: id },
        },
      });
    }).then(res => {
      if (res.success === true) {
        let olds = this.state.optionData;
        let obj = olds.find(c => c.id == id);
        if (obj) {
          let index = olds.indexOf(obj);
          let list = olds.slice();
          list.splice(index, 1);
          olds = list;
        }
        this.setState({
          optionData: [
            ...olds,
            {
              id,
              name,
              values: res.data,
            },
          ],
        });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleRemoveProductAttribute = record => {
    this.setState(({ productAttributeData }) => {
      const index = productAttributeData.indexOf(record);
      const list = productAttributeData.slice();
      list.splice(index, 1);
      return {
        productAttributeData: list,
      };
    });
  };

  handleRemoveProductOption = record => {
    this.setState(({ productOptionData }) => {
      const index = productOptionData.indexOf(record);
      const list = productOptionData.slice();
      list.splice(index, 1);
      return {
        productOptionData: list,
      };
    });
  };

  handleRemoveSku = record => {
    this.setState(({ productSku }) => {
      const index = productSku.indexOf(record);
      const list = productSku.slice();
      list.splice(index, 1);
      return {
        productSku: list,
      };
    });
  };

  handleInit = () => {
    const { dispatch } = this.props;
    this.setState({
      brandLoading: true,
      categoryLoading: true,
      templateLoading: true,
      attributeLoading: true,
      optionLoading: true,
      warehousesLoading: true,
      unitsLoading: true,
    });

    if (this.state.id) this.handleGetProduct();

    new Promise(resolve => {
      dispatch({
        type: 'catalog/units',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ unitsLoading: false });
      if (res.success === true) {
        this.setState({ units: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });

    new Promise(resolve => {
      dispatch({
        type: 'system/warehouses',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ warehousesLoading: false });
      if (res.success === true) {
        this.setState({ warehouses: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });

    new Promise(resolve => {
      dispatch({
        type: 'catalog/brands',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ brandLoading: false });
      if (res.success === true) {
        this.setState({ brands: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });

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

    new Promise(resolve => {
      dispatch({
        type: 'catalog/options',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ optionLoading: false });
      if (res.success === true) {
        this.setState({ options: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });

    new Promise(resolve => {
      dispatch({
        type: 'catalog/templates',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ templateLoading: false });
      if (res.success === true) {
        this.setState({ templates: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });

    new Promise(resolve => {
      dispatch({
        type: 'catalog/attributesGroupArray',
        payload: {
          resolve,
        },
      });
    }).then(res => {
      this.setState({ attributeLoading: false });
      if (res.success === true) {
        this.setState({ attributes: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleGetProduct = () => {
    const { dispatch } = this.props;
    this.setState({ loading: true });
    new Promise(resolve => {
      dispatch({
        type: 'product/get',
        payload: {
          resolve,
          params: { id: this.state.id },
        },
      });
    }).then(res => {
      this.setState({ loading: false });
      if (res.success === true) {
        this.setState(
          {
            current: res.data,
            currentIsPublished: res.data.isPublished,
            currentPublishType: res.data.publishType,
            currentStockTrackingIsEnabled: res.data.stockTrackingIsEnabled,
            isShipEnabled: res.data.isShipEnabled,
            productStocks: res.data.stocks,
          },
          () => {
            this.props.form.setFieldsValue({
              // shortDescription: BraftEditor.createEditorState(
              //   this.state.current.shortDescription || ''
              // ),
              description: BraftEditor.createEditorState(this.state.current.description || ''),
              specification: BraftEditor.createEditorState(this.state.current.specification || ''),
            });
          }
        );

        let imgs = res.data.productImages || [];
        let fs = [];
        imgs.forEach(c => {
          fs.push({
            uid: -c.id,
            name: c.caption || '',
            status: 'done',
            url: c.mediaUrl,
            mediaId: c.mediaId,
          });
          this.setState({ fileList: fs });
        });

        this.setState(
          {
            productAttributeData: res.data.attributes,
            productOptionData: res.data.options,
            productSku: res.data.variations,
          },
          () => {
            //加载属性对应的属性值列表
            this.state.productAttributeData.forEach(c => {
              this.queryAttributeData(c.id, c.name);
            });

            this.state.productOptionData.forEach(c => {
              this.queryOptionData(c.id, c.name);
            });
          }
        );
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleLoadStockHistory = () => {
    const { dispatch } = this.props;
    const params = {
      pagination: {
        current: this.state.pageNum,
        pageSize: this.state.pageSize,
      },
      sort: {
        predicate: this.state.predicate,
        reverse: this.state.reverse,
      },
      search: {
        productId: this.state.id,
      },
    };
    this.setState({ historyLoading: true });
    new Promise(resolve => {
      dispatch({
        type: 'product/stockHistories',
        payload: {
          resolve,
          params: params,
        },
      });
    }).then(res => {
      this.setState({ historyLoading: false });
      if (res.success === true) {
        this.setState({ historyData: res.data });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleUpload = file => {
    this.setState({ uploadLoading: true });

    const { dispatch } = this.props;

    const formData = new FormData();
    formData.append('file', file);

    // dispatch({
    //     type: 'upload/uploadImage',
    //     payload: {
    //         params: formData
    //     },
    // });
    // console.log(file);
    // console.log(uploadLoading);
    // return;

    new Promise(resolve => {
      dispatch({
        type: 'upload/uploadImage',
        payload: {
          resolve,
          params: formData,
        },
      });
    }).then(res => {
      this.setState({ uploadLoading: false });
      if (res.success === true) {
        let obj = this.state.fileList.find(c => c.mediaId == res.data.id);
        if (obj) {
          notification.warning({
            message: '图片已存在',
          });
          return;
        }

        file.url = res.data.url;
        file.mediaId = res.data.id;
        this.setState({
          fileList: [...this.state.fileList, file],
        });
      } else {
        notification.error({
          message: res.message,
        });
      }
    });
  };

  handleUploadMain = file => {
    this.setState({ uploadMainLoading: true });
    const { dispatch } = this.props;
    const formData = new FormData();
    formData.append('file', file);
    new Promise(resolve => {
      dispatch({
        type: 'upload/uploadImage',
        payload: {
          resolve,
          params: formData,
        },
      });
    }).then(res => {
      this.setState({ uploadMainLoading: false });
      if (res.success === true) {
        this.setState({
          current: Object.assign({}, this.state.current, {
            mediaId: res.data.id,
            mediaUrl: res.data.url,
          }),
        });
      } else {
        notification.error({ message: res.message });
      }
    });
  };

  handleRemove = file => {
    this.setState(({ fileList }) => {
      const index = fileList.indexOf(file);
      const newFileList = fileList.slice();
      newFileList.splice(index, 1);
      return {
        fileList: newFileList,
      };
    });
  };

  handleCancel = () => this.setState({ previewVisible: false });

  handlePreview = file => {
    this.setState({
      previewImage: file.url || file.thumbUrl || file.mediaUrl,
      previewVisible: true,
    });
  };

  // handleUploadChange = info => {
  //     const status = info.file.status;
  //     if (status !== 'uploading') {
  //         console.log(info.file, info.fileList);
  //     }
  //     if (status === 'done') {
  //         console.log(`${info.file.name} file uploaded successfully.`);
  //     } else if (status === 'error') {
  //         console.log(`${info.file.name} file upload failed.`);
  //     }
  // }

  handleHistoryStandardTableChange = (pagination, filtersArg, sorter) => {
    this.setState(
      {
        pageNum: pagination.current,
        pageSize: pagination.pageSize,
        search: {
          ...filtersArg,
        },
      },
      () => {
        if (sorter.field) {
          this.setState(
            {
              predicate: sorter.field,
              reverse: sorter.order == 'descend',
            },
            () => {
              this.handleLoadStockHistory();
            }
          );
        } else {
          this.handleLoadStockHistory();
        }
      }
    );
  };

  //https://www.yuque.com/braft-editor/be/gz44tn
  myUploadFn = param => {
    // console.log(param);
    const { dispatch } = this.props;
    const fd = new FormData();
    fd.append('file', param.file);
    new Promise(resolve => {
      dispatch({
        type: 'upload/uploadImage',
        payload: {
          resolve,
          params: fd,
        },
      });
    }).then(res => {
      // console.log(res);
      if (res.success === true) {
        // 上传进度发生变化时调用param.progress
        // param.progress(100)
        param.success({
          url: res.data.url,
          meta: {
            id: res.data.id,
            title: res.data.fileName,
            alt: res.data.fileName,
            // loop: true, // 指定音视频是否循环播放
            // autoPlay: true, // 指定音视频是否自动播放
            // controls: true, // 指定音视频是否显示控制栏
            poster: res.data.url, // 指定视频播放器的封面
          },
        });
      } else {
        notification.error({ message: res.message });
        param.error({
          msg: 'unable to upload.',
        });
      }
    });
  };

  render() {
    const {
      editorState,
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
    const itemFormLayout = {
      labelCol: { span: 7 },
      wrapperCol: { span: 13 },
    };
    const submitFormLayout = {
      wrapperCol: {
        xs: { span: 24, offset: 0 },
        sm: { span: 10, offset: 4 },
      },
    };

    const { previewVisible, previewImage } = this.state;
    const uploadButton = (
      <div>
        <Icon type={this.state.uploadLoading ? 'loading' : 'plus'} />
        <div className="ant-upload-text">上传</div>
      </div>
    );
    const controls = [
      'headings',
      'font-size',
      'separator',
      'bold',
      'italic',
      'underline',
      'text-color',
      'strike-through',
      'emoji',
      'media',
      'separator',
      'link',
      'separator',
      'text-indent',
      'text-align',
      'separator',
      'list-ul',
      'list-ol',
      'blockquote',
      'code',
      'hr',
      'separator',

      'remove-styles',
      'fullscreen',
      'clear',
    ];
    const controlsEasy = [
      'bold',
      'italic',
      'underline',
      'text-color',
      'media',
      'separator',
      'link',
      'separator',
      'text-align',
      'separator',
      'list-ul',
      'list-ol',
      'separator',
      'remove-styles',
    ];

    const historyPagination = {
      showQuickJumper: true,
      showSizeChanger: true,
      pageSizeOptions: ['5', '10', '50', '100'],
      defaultPageSize: this.state.pageSize,
      defaultCurrent: this.state.pageNum,
      current: this.state.pageNum,
      pageSize: this.state.pageSize,
      total: this.state.historyData.pagination.total || 0,
      showTotal: (total, range) => {
        return `${range[0]}-${range[1]} 条 , 共 ${total} 条`;
      },
    };

    const rollback = this.state.id ? (
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
        <Button type="primary" icon="copy" onClick={this.showCopyModal}>
          复制商品
        </Button>
        <Button type="danger" icon="delete" onClick={this.showDeleteModal}>
          删除
        </Button>
        <Link to="./list">
          <Button>
            <Icon type="rollback" />
          </Button>
        </Link>
      </Fragment>
    ) : (
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

    return (
      <PageHeaderWrapper title={this.state.id ? '编辑商品' : '新增商品'} action={rollback}>
        <Spin spinning={this.state.loading}>
          <Card bordered={false}>
            <Form onSubmit={this.handleSubmit} style={{ marginTop: 8 }}>
              <Tabs type="card" onChange={this.handleTabChange}>
                <TabPane tab="基本信息" key="1">
                  <FormItem {...formItemLayout} label={<span>名称</span>}>
                    {getFieldDecorator('name', {
                      initialValue: this.state.current.name || '',
                      rules: [{ required: true, message: '请输入商品名称' }],
                    })(
                      <Input
                        onChange={e => {
                          this.setState({
                            current: Object.assign({}, this.state.current, {
                              name: e.target.value,
                            }),
                          });
                        }}
                        placeholder="名称"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>Slug</span>}>
                    {getFieldDecorator('slug', {
                      rules: [
                        {
                          required: true,
                        },
                      ],
                      initialValue: this.state.current.slug || '',
                    })(<Input placeholder="Slug" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>品牌</span>}>
                    {getFieldDecorator('brandId', {
                      initialValue: this.state.current.brandId || '',
                    })(
                      <Select loading={this.state.brandLoading} allowClear={true}>
                        {this.state.brands.map(c => {
                          return (
                            <Option value={c.id} key={c.id}>
                              {c.name}
                            </Option>
                          );
                        })}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>简短描述</span>}>
                    {getFieldDecorator('shortDescription', {
                      initialValue: this.state.current.shortDescription || '',
                    })(
                      <TextArea rows={3} />
                      // <BraftEditor
                      //   className={styles.myEditor}
                      //   controls={controlsEasy}
                      //   placeholder=""
                      //   contentStyle={{ height: 100 }}
                      // />
                    )}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={
                      <span>
                        规格描述
                        <Tooltip
                          placement="topLeft"
                          title="规格是商品的非销售属性，例如：屏幕尺寸，USB端口数量。"
                        >
                          <Icon type="question-circle" theme="filled" />
                        </Tooltip>
                      </span>
                    }
                  >
                    {getFieldDecorator('specification')(
                      <BraftEditor
                        className={styles.myEditor}
                        controls={controlsEasy}
                        placeholder=""
                        contentStyle={{ height: 100 }}
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>描述</span>}>
                    {getFieldDecorator('description')(
                      <BraftEditor
                        media={{ uploadFn: this.myUploadFn }}
                        className={styles.myEditor}
                        controls={controls}
                        placeholder=""
                        contentStyle={{ height: 200 }}
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>SKU</span>}>
                    {getFieldDecorator('sku', { initialValue: this.state.current.sku || '' })(
                      <Input placeholder="SKU" />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>GTIN</span>}>
                    {getFieldDecorator('gtin', { initialValue: this.state.current.gtin || '' })(
                      <Input
                        onChange={e => {
                          this.setState({
                            current: Object.assign({}, this.state.current, {
                              gtin: e.target.value,
                            }),
                          });
                        }}
                        placeholder="GTIN"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品条形码</span>}>
                    {getFieldDecorator('barcode', {
                      initialValue: this.state.current.barcode || '',
                    })(<Input placeholder="商品条形码" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>单位</span>}>
                    {getFieldDecorator('unitId', {
                      initialValue: this.state.current.unitId || '',
                      valuePropName: 'value',
                    })(
                      <Select
                        placeholder="单位"
                        loading={this.state.unitsLoading}
                        allowClear={true}
                      >
                        {this.state.units.map(c => {
                          return (
                            <Option value={c.id} key={c.id}>
                              {c.name}
                            </Option>
                          );
                        })}
                      </Select>
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>价格</span>}>
                    {getFieldDecorator('price', {
                      rules: [{ required: true, message: '请输入商品价格' }],
                      initialValue: this.state.current.price || 0,
                    })(
                      <InputNumber
                        min={0}
                        onChange={e => {
                          this.setState({
                            current: Object.assign({}, this.state.current, { price: e }),
                          });
                        }}
                        style={{ width: '100%' }}
                        placeholder="价格"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>原价</span>}>
                    {getFieldDecorator('oldPrice', { initialValue: this.state.current.oldPrice })(
                      <InputNumber
                        min={0}
                        onChange={e => {
                          this.setState({
                            current: Object.assign({}, this.state.current, { oldPrice: e }),
                          });
                        }}
                        style={{ width: '100%' }}
                        placeholder="原价"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>特价</span>}>
                    {getFieldDecorator('specialPrice', {
                      initialValue: this.state.current.specialPrice,
                    })(<InputNumber min={0} style={{ width: '100%' }} placeholder="特价" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>特价时间</span>}>
                    {getFieldDecorator('specialPriceRangePicker', {
                      initialValue:
                        this.state.current.specialPriceStart && this.state.current.specialPriceEnd
                          ? [
                              moment(this.state.current.specialPriceStart, 'YYYY/MM/DD HH:mm:ss'),
                              moment(this.state.current.specialPriceEnd, 'YYYY/MM/DD HH:mm:ss'),
                            ]
                          : [],
                    })(
                      <RangePicker
                        ranges={{
                          Today: [moment(), moment()],
                          'This Month': [moment().startOf('month'), moment().endOf('month')],
                        }}
                        showTime
                        format="YYYY/MM/DD HH:mm:ss"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品主图</span>}>
                    <Upload
                      action={this.handleUploadMain}
                      listType="picture-card"
                      showUploadList={false}
                      // onChange={this.handleChange}
                      // onPreview={this.handlePreview}
                    >
                      <Spin spinning={this.state.uploadMainLoading}>
                        {this.state.current.mediaId ? (
                          <img height={102} src={this.state.current.mediaUrl} />
                        ) : (
                          <div>
                            <Icon type={this.state.uploadMainLoading ? 'loading' : 'plus'} />
                            <div className="ant-upload-text">上传</div>
                          </div>
                        )}
                      </Spin>
                    </Upload>
                    {this.state.current.mediaId ? (
                      <Button
                        onClick={() => {
                          this.setState({
                            current: Object.assign({}, this.state.current, {
                              mediaId: '',
                              mediaUrl: '',
                            }),
                          });
                        }}
                        icon="close"
                        size="small"
                      />
                    ) : null}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品图片</span>}>
                    <Upload
                      action={this.handleUpload}
                      listType="picture-card"
                      fileList={this.state.fileList}
                      onRemove={this.handleRemove}
                      onPreview={this.handlePreview}
                      // onChange={this.handleUploadChange}
                    >
                      {uploadButton}
                    </Upload>
                    <Modal visible={previewVisible} footer={null} onCancel={this.handleCancel}>
                      <img alt="image" style={{ width: '100%' }} src={previewImage} />
                    </Modal>
                  </FormItem>

                  <FormItem
                    {...formItemLayout}
                    label={
                      <span>
                        单独可见
                        <Tooltip
                          placement="topLeft"
                          title="如果你想让这个商品在目录或搜索结果中可以看到，请开启它。仅商品组合允许设置此值。"
                        >
                          <Icon type="question-circle" theme="filled" />
                        </Tooltip>
                      </span>
                    }
                  >
                    {getFieldDecorator('isVisibleIndividually', {
                      initialValue: this.state.current.isVisibleIndividually || false,
                      valuePropName: 'checked',
                    })(
                      <Checkbox disabled={(this.state.current.parentGroupedProductId || 0) <= 0} />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>允许订购</span>}>
                    {getFieldDecorator('isAllowToOrder', {
                      initialValue: this.state.current.isAllowToOrder || false,
                      valuePropName: 'checked',
                    })(<Checkbox />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>精品</span>}>
                    {getFieldDecorator('isFeatured', {
                      initialValue: this.state.current.isFeatured || false,
                      valuePropName: 'checked',
                    })(<Checkbox />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>已发布</span>}>
                    {getFieldDecorator('isPublished', {
                      initialValue: this.state.current.isPublished || false,
                      valuePropName: 'checked',
                    })(
                      <Checkbox
                        // disabled
                        onChange={e => {
                          this.setState({ currentIsPublished: e.target.checked });
                          if (e.target.checked == false) {
                            this.setState({ currentPublishType: 0 });
                          }
                        }}
                      />
                    )}
                    {!this.state.currentIsPublished ? (
                      <Card type="inner">
                        <FormItem {...itemFormLayout} label="">
                          {getFieldDecorator('publishType', {
                            initialValue: this.state.currentPublishType || 0,
                          })(
                            <Radio.Group
                              onChange={e => {
                                this.setState({ currentPublishType: e.target.value });
                              }}
                            >
                              <Radio value={0}>立即发布</Radio>
                              <Radio value={1}>定时发布</Radio>
                            </Radio.Group>
                          )}
                        </FormItem>
                        {this.state.currentPublishType == 1 ? (
                          <FormItem>
                            {getFieldDecorator('publishedOn', {
                              initialValue: this.state.current.publishedOn
                                ? moment(this.state.current.publishedOn, 'YYYY/MM/DD HH:mm:ss')
                                : null,
                              valuePropName: 'defaultValue',
                            })(
                              <DatePicker
                                showTime
                                format="YYYY/MM/DD HH:mm:ss"
                                placeholder="定时发布时间"
                              />
                            )}
                          </FormItem>
                        ) : null}
                        {this.state.current.unpublishedOn ? (
                          <div>
                            取消发布时间：
                            {this.state.current.unpublishedOn}
                          </div>
                        ) : null}
                        {this.state.current.unpublishedReason ? (
                          <div>
                            取消发布原因：
                            {this.state.current.unpublishedReason}
                          </div>
                        ) : null}
                      </Card>
                    ) : this.state.current.publishedOn ? (
                      <div>
                        发布时间：
                        {this.state.current.publishedOn}
                      </div>
                    ) : null}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={
                      <span>
                        商品有效期
                        <Tooltip
                          placement="topLeft"
                          title="商品有效期。单位:天。自发布/上架时间起计算，如果过期，则自动取消发布/下架。发布时，计算下架时间。默认：长期有效"
                        >
                          <Icon type="question-circle" theme="filled" />
                        </Tooltip>
                      </span>
                    }
                  >
                    {getFieldDecorator('validThru', { initialValue: this.state.current.validThru })(
                      <InputNumber
                        min={0}
                        precision={0}
                        style={{ width: '100%' }}
                        placeholder="商品有效期"
                      />
                    )}
                  </FormItem>
                  <FormItem {...formItemLayout} label="启用库存跟踪">
                    {getFieldDecorator('stockTrackingIsEnabled', {
                      initialValue: this.state.current.stockTrackingIsEnabled || false,
                      valuePropName: 'checked',
                    })(
                      <Checkbox
                        onChange={e => {
                          this.setState({ currentStockTrackingIsEnabled: e.target.checked });
                        }}
                      />
                    )}
                    {this.state.currentStockTrackingIsEnabled ? (
                      <Card type="inner" title="库存跟踪">
                        <FormItem {...itemFormLayout} label="显示库存可用性">
                          {getFieldDecorator('displayStockAvailability', {
                            initialValue: this.state.current.displayStockAvailability || false,
                            valuePropName: 'checked',
                          })(<Checkbox />)}
                        </FormItem>
                        <FormItem {...itemFormLayout} label="显示库存量">
                          {getFieldDecorator('displayStockQuantity', {
                            initialValue: this.state.current.displayStockQuantity || false,
                            valuePropName: 'checked',
                          })(<Checkbox />)}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>最低购物车数量</span>}>
                          {getFieldDecorator('orderMinimumQuantity', {
                            initialValue: this.state.current.orderMinimumQuantity || 1,
                          })(
                            <InputNumber
                              min={1}
                              precision={0}
                              style={{ width: '100%' }}
                              placeholder="最低购物车数量"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>最大购物车数量</span>}>
                          {getFieldDecorator('orderMaximumQuantity', {
                            initialValue: this.state.current.orderMaximumQuantity || 1000,
                          })(
                            <InputNumber
                              min={1}
                              precision={0}
                              style={{ width: '100%' }}
                              placeholder="最大购物车数量"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label="不可退货">
                          {getFieldDecorator('notReturnable', {
                            initialValue: this.state.current.notReturnable || false,
                            valuePropName: 'checked',
                          })(<Checkbox />)}
                        </FormItem>
                        <FormItem {...itemFormLayout} label="库存扣减策略">
                          {getFieldDecorator('stockReduceStrategy', {
                            initialValue: this.state.current.stockReduceStrategy || 0,
                          })(
                            <Radio.Group>
                              <Radio value={0}>下单减库存</Radio>
                              <Radio value={1}>支付减库存</Radio>
                            </Radio.Group>
                          )}
                        </FormItem>
                        <FormItem
                          {...itemFormLayout}
                          // label={<span>有效库存数量</span>}
                          label={
                            <span>
                              有效库存数量
                              <Tooltip placement="topLeft" title="可用仓库，可用库存数量">
                                <Icon type="question-circle" theme="filled" />
                              </Tooltip>
                            </span>
                          }
                        >
                          {getFieldDecorator('stockQuantity', {
                            initialValue: this.state.current.stockQuantity || 0,
                          })(
                            <InputNumber
                              disabled
                              min={0}
                              precision={0}
                              style={{ width: '100%' }}
                              placeholder="库存数量"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>仓库</span>}>
                          {getFieldDecorator('warehouseIds', {
                            initialValue: this.state.current.warehouseIds,
                            valuePropName: 'value',
                          })(
                            <Select
                              mode="multiple"
                              placeholder="仓库"
                              loading={this.state.warehousesLoading}
                              allowClear={true}
                              onChange={e => {
                                this.setState({ productStocksLoading: true });
                                let stocks = this.state.productStocks;
                                e.forEach(c => {
                                  let first = this.state.warehouses.find(x => x.id == c);
                                  if (first && stocks.findIndex(x => x.id == c) < 0) {
                                    let s = {
                                      id: first.id,
                                      name: first.name,
                                      quantity: 0,
                                      displayOrder: 0,
                                      isEnabled: true,
                                    };
                                    stocks.push(s);
                                  }
                                });
                                this.setState({
                                  productStocks: stocks.filter(c => e.indexOf(c.id) >= 0),
                                  productStocksLoading: false,
                                });
                              }}
                            >
                              {this.state.warehouses.map(c => {
                                return (
                                  <Option value={c.id} key={c.id}>
                                    {c.name}
                                  </Option>
                                );
                              })}
                            </Select>
                          )}
                        </FormItem>
                        <Table
                          bordered={true}
                          rowKey={(record, index) => `product_stock_${record.id}_i_${index}`} //{record => record.id}
                          pagination={false}
                          loading={this.state.productStocksLoading}
                          dataSource={this.state.productStocks}
                          columns={this.columnsProductStock}
                          // scroll={{ x: 360 }}
                        />
                      </Card>
                    ) : null}
                  </FormItem>
                  <FormItem {...formItemLayout} label="启用航运">
                    {getFieldDecorator('isShipEnabled', {
                      initialValue: this.state.current.isShipEnabled || false,
                      valuePropName: 'checked',
                    })(
                      <Checkbox
                        onChange={e => {
                          this.setState({ isShipEnabled: e.target.checked });
                        }}
                      />
                    )}
                    {this.state.isShipEnabled ? (
                      <Card type="inner" title="配送">
                        <FormItem {...itemFormLayout} label={<span>重量(kg)</span>}>
                          {getFieldDecorator('weight', {
                            initialValue: this.state.current.weight || 0,
                          })(
                            <InputNumber
                              min={0}
                              precision={3}
                              style={{ width: '100%' }}
                              placeholder="重量"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>长度(cm)</span>}>
                          {getFieldDecorator('length', {
                            initialValue: this.state.current.length || 0,
                          })(
                            <InputNumber
                              min={0}
                              precision={2}
                              style={{ width: '100%' }}
                              placeholder="长度"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>宽度(cm)</span>}>
                          {getFieldDecorator('width', {
                            initialValue: this.state.current.width || 0,
                          })(
                            <InputNumber
                              min={0}
                              precision={2}
                              style={{ width: '100%' }}
                              placeholder="宽度"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>高度(m)</span>}>
                          {getFieldDecorator('height', {
                            initialValue: this.state.current.height || 0,
                          })(
                            <InputNumber
                              min={0}
                              precision={2}
                              style={{ width: '100%' }}
                              placeholder="高度"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label="是否免费配送">
                          {getFieldDecorator('isFreeShipping', {
                            initialValue: this.state.current.isFreeShipping || false,
                            valuePropName: 'checked',
                          })(<Checkbox />)}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>额外运费</span>}>
                          {getFieldDecorator('additionalShippingCharge', {
                            initialValue: this.state.current.additionalShippingCharge || 0,
                          })(
                            <InputNumber
                              min={0}
                              precision={2}
                              style={{ width: '100%' }}
                              placeholder="额外运费"
                            />
                          )}
                        </FormItem>
                        <FormItem {...itemFormLayout} label={<span>运费模板</span>}>
                          {getFieldDecorator('warehouseId', {
                            initialValue: this.state.current.warehouseId || '',
                            valuePropName: 'value',
                          })(
                            <Select
                              placeholder="运费模板"
                              loading={this.state.warehousesLoading}
                              allowClear={true}
                            >
                              {this.state.warehouses.map(c => {
                                return (
                                  <Option value={c.id} key={c.id}>
                                    {c.name}
                                  </Option>
                                );
                              })}
                            </Select>
                          )}
                        </FormItem>
                        <FormItem
                          {...itemFormLayout}
                          label={
                            <span>
                              备货期
                              <Tooltip
                                placement="topLeft"
                                title="备货期。发货天数。取值范围:1-60;单位:天。"
                              >
                                <Icon type="question-circle" theme="filled" />
                              </Tooltip>
                            </span>
                          }
                        >
                          {getFieldDecorator('deliveryTime', {
                            initialValue: this.state.current.deliveryTime,
                          })(
                            <InputNumber
                              min={0}
                              precision={0}
                              style={{ width: '100%' }}
                              placeholder="备货期"
                            />
                          )}
                        </FormItem>
                      </Card>
                    ) : null}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>isCallForPricing</span>}>
                    {getFieldDecorator('isCallForPricing', {
                      initialValue: this.state.current.isCallForPricing || false,
                      valuePropName: 'checked',
                    })(<Checkbox />)}
                  </FormItem>
                  <FormItem
                    {...formItemLayout}
                    label={<span>管理员备注</span>}
                    help={<FormattedMessage id="form.admin.remark.help" />}
                  >
                    {getFieldDecorator('adminRemark', {
                      initialValue: this.state.current.adminRemark || '',
                    })(<TextArea style={{ minHeight: 32 }} placeholder="" rows={2} />)}
                  </FormItem>
                </TabPane>
                <TabPane
                  tab="商品选项"
                  // tab={<span>商品选项
                  //         <Tooltip placement="topLeft" title="销售属性，例如：尺码，颜色。">
                  //         <Icon type="question-circle" theme="filled" />
                  //     </Tooltip>
                  // </span>}
                  disabled={(this.state.current.parentGroupedProductId || 0) > 0}
                  key="2"
                >
                  <FormItem {...formItemLayout} label={<span>可用选项</span>}>
                    <Select
                      labelInValue
                      placeholder="可用选项"
                      loading={this.state.optionLoading}
                      allowClear={true}
                      onChange={value => this.setState({ optionCurrent: value })}
                    >
                      {this.state.options.map(c => {
                        return <Option key={c.id}>{c.name}</Option>;
                      })}
                    </Select>
                    <Button onClick={this.handleAddProductOption}>添加选项</Button>
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品选项</span>}>
                    <Table
                      bordered={false}
                      rowKey={record => record.id}
                      pagination={false}
                      loading={this.state.productOptionDataLoading}
                      dataSource={this.state.productOptionData}
                      columns={this.columnsOption}
                    />
                    <Button onClick={this.handleGenerateOptionCombination}>生成组合</Button>
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品组合</span>}>
                    <Table
                      bordered={false}
                      rowKey={(record, index) => `sku_${record.id}_i_${index}`} //{record => record.id}
                      pagination={false}
                      loading={this.state.productSkuLoading}
                      dataSource={this.state.productSku}
                      columns={this.columnsSku}
                      scroll={{ x: 960 }}
                    />
                    <Button onClick={this.handleAddOptionCombination}>添加组合</Button>
                  </FormItem>
                </TabPane>
                <TabPane
                  tab="商品属性"
                  // tab={<span>商品属性
                  // <Tooltip placement="topLeft" title="非销售属性，例如：材质，季节。">
                  //         <Icon type="question-circle" theme="filled" />
                  //     </Tooltip>
                  // </span>}
                  key="3"
                >
                  <FormItem {...formItemLayout} label={<span>属性模板</span>}>
                    <Select
                      placeholder="属性模板"
                      loading={this.state.templateLoading}
                      allowClear={true}
                      onChange={value => this.setState({ templateCurrent: value })}
                    >
                      {this.state.templates.map(c => {
                        return <Option key={c.id}>{c.name}</Option>;
                      })}
                    </Select>
                    <Button
                      loading={this.state.applyLoading}
                      onClick={this.handleApplyProductAttrTemplate}
                    >
                      应用
                    </Button>
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>可用属性</span>}>
                    <Select
                      labelInValue
                      placeholder="可用属性"
                      loading={this.state.attributeLoading}
                      allowClear={true}
                      onChange={value => this.setState({ attributeCurrent: value })}
                    >
                      {this.state.attributes.map(x => {
                        if (x.productAttributes) {
                          let options = x.productAttributes.map(c => {
                            return (
                              <Option value={c.id} key={c.id}>
                                {c.name}
                              </Option>
                            );
                          });
                          return (
                            <OptGroup key={x.groupId} label={x.groupName}>
                              {options}
                            </OptGroup>
                          );
                        }
                      })}
                    </Select>
                    <Button onClick={this.handleAddProductAttribute}>添加属性</Button>
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>商品属性</span>}>
                    <Table
                      bordered={false}
                      rowKey={record => record.id}
                      pagination={false}
                      loading={this.state.productAttributeLoading}
                      dataSource={this.state.productAttributeData}
                      columns={this.columnsAttribute}
                    />
                  </FormItem>
                </TabPane>
                <TabPane tab="商品类别" key="4">
                  <FormItem {...formItemLayout} label={<span>商品类别映射</span>}>
                    {getFieldDecorator('categoryIds', {
                      initialValue: this.state.current.categoryIds || [],
                      valuePropName: 'value',
                    })(
                      <Select mode="multiple" placeholder="请选择商品类别" allowClear={true}>
                        {this.state.categories.map(c => {
                          return (
                            <Option value={c.id} key={c.id}>
                              {c.name}
                            </Option>
                          );
                        })}
                      </Select>
                    )}
                  </FormItem>
                </TabPane>
                <TabPane tab="SEO" key="5">
                  <FormItem {...formItemLayout} label={<span>Meta Title</span>}>
                    {getFieldDecorator('metaTitle', {
                      initialValue: this.state.current.metaTitle || '',
                    })(<Input placeholder="Meta Title" />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>Meta Keywords</span>}>
                    {getFieldDecorator('metaKeywords', {
                      initialValue: this.state.current.metaKeywords || '',
                    })(<TextArea style={{ minHeight: 32 }} placeholder="Meta Keywords" rows={2} />)}
                  </FormItem>
                  <FormItem {...formItemLayout} label={<span>Meta Description</span>}>
                    {getFieldDecorator('metaDescription', {
                      initialValue: this.state.current.metaDescription || '',
                    })(
                      <TextArea style={{ minHeight: 32 }} placeholder="Meta Description" rows={2} />
                    )}
                  </FormItem>
                </TabPane>
                <TabPane tab="库存历史" key="6">
                  <Card bordered={false}>
                    <StandardTable
                      pagination={historyPagination}
                      loading={this.state.historyLoading}
                      data={this.state.historyData}
                      rowKey={record => record.id}
                      columns={this.columnsHistory}
                      bordered
                      onChange={this.handleHistoryStandardTableChange}
                      // scroll={{ x: 1500 }}
                    />
                  </Card>
                </TabPane>
                <TabPane tab="购买记录" key="7" />
                <TabPane tab="操作记录" key="8" />
              </Tabs>
              {/* <FormItem {...submitFormLayout}>
                                <Button type="primary" htmlType="submit" loading={this.state.submitting}>保存</Button>
                            </FormItem> */}
            </Form>
          </Card>
        </Spin>
        <Modal
          width={600}
          title={`选项配置 - ${this.state.optionSettingCurrent.name}`}
          destroyOnClose
          visible={this.state.visibleOptionSetting}
          footer={null}
          onCancel={this.handleOptionSettingCancel}
        >
          <Table
            bordered={false}
            rowKey={(record, index) => `option_${record.id}_v_${index}`} //{record => record.id}
            pagination={false}
            dataSource={this.state.optionSettingCurrent.values}
            columns={this.columnsOptionSetting}
          />
        </Modal>
        <Modal
          // width={600}
          title={`库存配置 - ${this.state.skuStockCurrent.name}`}
          destroyOnClose
          visible={this.state.visibleSkuStocks}
          footer={null}
          onCancel={this.handleSkuStocksCancel}
        >
          <Select
            style={{ width: '100%', marginBottom: 16 }}
            mode="multiple"
            placeholder="仓库"
            defaultValue={this.state.skuStockCurrent.warehouseIds || []}
            loading={this.state.warehousesLoading}
            allowClear={true}
            onChange={e => {
              let current = this.state.skuStockCurrent;
              let stocks = current.stocks || [];
              e.forEach(c => {
                let first = this.state.warehouses.find(x => x.id == c);
                if (first && stocks.findIndex(x => x.id == c) < 0) {
                  let s = {
                    id: first.id,
                    name: first.name,
                    quantity: 0,
                    displayOrder: 0,
                    isEnabled: true,
                    type: 'sku',
                  };
                  stocks.push(s);
                }
              });

              let record = {};
              if (current.id) {
                record = this.state.productSku.find(c => c.id == current.id);
              } else {
                record = this.state.productSku.find(c => c.name == current.name);
              }
              if (record) {
                let index = this.state.productSku.indexOf(record);
                if (index >= 0) {
                  let list = this.state.productSku.slice();
                  list.splice(index, 1);

                  record.stocks = stocks.filter(c => e.indexOf(c.id) >= 0);
                  record.warehouseIds = e;

                  list.splice(index, 0, record);
                  this.setState({
                    productSku: list,
                    skuStockCurrent: record,
                  });
                }
              }
            }}
          >
            {this.state.warehouses.map(c => {
              return (
                <Option value={c.id} key={c.id}>
                  {c.name}
                </Option>
              );
            })}
          </Select>
          <Table
            bordered={false}
            rowKey={(record, index) => `sku_stock_${record.id}_v_${index}`} //{record => record.id}
            pagination={false}
            dataSource={this.state.skuStockCurrent.stocks || []}
            columns={this.columnsProductStock}
          />
        </Modal>
        <Modal
          title={`添加组合`}
          destroyOnClose
          visible={this.state.visibleOptionAdd}
          onCancel={this.handleAddOptionCombinationCancel}
          onOk={this.handleAddOptionCombinationOk}
        >
          {this.state.visibleOptionAdd
            ? this.state.productOptionData.map(c => {
                return (
                  <Select
                    onChange={v => {
                      let obj = this.state.addOptionCombination.find(x => x.id == c.id);
                      if (obj) {
                        obj.value = v;
                      }
                    }}
                    key={c.name}
                    style={{ width: '60%', marginBottom: 10 }}
                    placeholder={c.name}
                  >
                    {c.values.map(x => {
                      return <Option key={x.value}>{x.value}</Option>;
                    })}
                  </Select>
                );
              })
            : null}
        </Modal>
        <CopyCommponent
          visible={this.state.visibleCopy}
          current={this.state.current}
          wrappedComponentRef={this.saveFormRef}
          onCancel={this.handleCopyCancel}
          onOk={this.handleCopySubmit}
          copySubmitting={this.state.copySubmitting}
        />
      </PageHeaderWrapper>
    );
  }
}

export default ProductInfo;
