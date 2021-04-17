import React from 'react';
import { Input, Modal, Checkbox, Form, Button, Alert } from 'antd';

const FormItem = Form.Item;

@Form.create()
class CopyCommponent extends React.Component {
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
                <Modal
                    title={`复制商品`}
                    destroyOnClose
                    visible={this.props.visible}
                    onCancel={this.props.onCancel}
                    onOk={this.props.onOk}
                    // footer={[
                    //     <Button onClick={this.props.onCancel}>取消</Button>,
                    //     <Button type="primary" loading={this.props.copySubmitting} onClick={this.props.onOk}>
                    //         确定
                    //     </Button>,
                    // ]}
                >
                    <Alert style={{ marginBottom: 24 }}
                        message="提示：不复制商品组合" type="info" showIcon />
                    {
                        <Form>
                            <FormItem
                                {...formItemLayout}
                                label={<span>名称</span>}>
                                {getFieldDecorator('name', {
                                    initialValue: this.props.current.name || '',
                                    rules: [{ required: true, message: '请输入商品名称' }],
                                })(
                                    <Input placeholder="名称" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>Slug</span>}>
                                {getFieldDecorator('slug',
                                    {
                                        initialValue: this.props.current.slug || '',
                                        rules: [{ required: true }],
                                    })(<Input placeholder="Slug" />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>复制图片</span>}>
                                {getFieldDecorator('isCopyImages', {
                                    initialValue: true, valuePropName: 'checked'
                                })(<Checkbox />)}
                            </FormItem>
                            <FormItem
                                {...formItemLayout}
                                label={<span>复制库存</span>}>
                                {getFieldDecorator('isCopyStock', {
                                    initialValue: true, valuePropName: 'checked'
                                })(<Checkbox />)}
                            </FormItem>
                        </Form>
                    }
                </Modal>
            </div>
        );
    }
}

export default CopyCommponent;
