const merge = require('webpack-merge');
const base = require('./webpack.base');

module.exports = merge(base, {
  mode: 'development',
  watch: true,
  devtool: 'cheap-eval-source-map',
  devServer: {
    port: 9000,
    color: true,
    host: '0.0.0.0',
    hot: true,
    publicPath: '/wds/',
    proxy: {
      '*': 'https://localhost:5001/'
    }
  }
})