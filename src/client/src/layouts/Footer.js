import React, { Fragment } from 'react';
import { Layout, Icon } from 'antd';
import GlobalFooter from '@/components/GlobalFooter';

const { Footer } = Layout;
const FooterView = () => (
  <Footer style={{ padding: 0 }}>
    <GlobalFooter
      links={[
        {
          key: 'Admin Web',
          title: <span><Icon type="github" />Admin Web</span>,
          href: 'https://github.com/trueai-org/module-shop-admin-web',
          blankTarget: true,
        },
        {
          key: 'Api',
          title: <span><Icon type="github" />Api</span>,
          href: 'https://github.com/trueai-org/module-shop-api',
          blankTarget: true,
        },
        {
          key: 'Mini Program',
          title: <span><Icon type="github" />Mini Program</span>,
          href: 'https://github.com/trueai-org/module-shop-mini-program',
          blankTarget: true,
        },
      ]}
      copyright={
        <Fragment>
          Copyright <Icon type="copyright" /> 2019 <a href="https://trueai.org/" target="_blank">TRUEAI.ORG</a>
        </Fragment>
      }
    />
  </Footer>
);
export default FooterView;
