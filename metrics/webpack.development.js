const merge = require('webpack-merge');
const base = require('./webpack.base');
const webpack = require('webpack');

module.exports = merge(base, {
  mode: 'development',
  output: {
    publicPath: '/wds/'
  },
  devtool: 'cheap-eval-source-map',
  devServer: {
    port: 9001,
    host: '0.0.0.0',
    hot: true,
    proxy: {
      '*': {
        target: 'http://localhost:5000/',
        changeOrigin: true
      }
    }
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin()
  ]
})