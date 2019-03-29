const merge = require('webpack-merge');
const baseWebpackConfig = require('./webpack.base.js');

const webpackConfig = {
  merge(baseWebpackConfig, (
    require(`./webpack.${process.env.NODE_ENV}.js`)
  );
};