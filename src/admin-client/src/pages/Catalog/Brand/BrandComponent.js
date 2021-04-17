import React from 'react';
import { Input, Modal, Checkbox, Form, Button, } from 'antd';

const FormItem = Form.Item;

@Form.create()
class BrandComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
    }
    componentDidMount() { }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: {
                xs: { span: 24 },
                sm: { span: 6 },
            },
            wrapperCol: {
                xs: { span: 24 },
                sm: { span: 16 },
            },
        };
        return (
            <div>
                <Modal title={this.props.brand.id ? '编辑品牌' : '添加品牌'}
                    visible={this.props.visible}
                    onOk={this.props.onCreate}
                    onCancel={this.props.handleCancel}>
                    <Form>
                        <FormItem>
                            {getFieldDecorator('id', { initialValue: this.props.brand.id, })(<Input type="hidden" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>名称</span>}>
                            {getFieldDecorator('name', {
                                initialValue: this.props.brand.name,
                                rules: [{ required: true, message: '请输入品牌名称' }],
                            })(<Input placeholder="名称" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>Slug</span>}>
                            {getFieldDecorator('slug', {
                                initialValue: this.props.brand.slug,
                            })(<Input placeholder="Slug" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>描述</span>}>
                            {getFieldDecorator('description', {
                                initialValue: this.props.brand.description,
                            })(<Input placeholder="描述" />)}
                        </FormItem>
                        <FormItem
                            {...formItemLayout}
                            label={<span>发布</span>}>
                            {getFieldDecorator('isPublished', {
                                initialValue: this.props.brand.isPublished || false, valuePropName: 'checked'
                            })(<Checkbox />)}
                        </FormItem>
                    </Form>
                </Modal>
            </div>
        );
    }
}

export default BrandComponent;
