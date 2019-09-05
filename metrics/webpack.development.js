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
    port: 9000,
    host: '0.0.0.0',
    hot: true,
    proxy: {
      '*': {
        target: 'https://localhost:5001/',
        changeOrigin: true,
        secure: false
      }
    }
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin(),
    new webpack.EnvironmentPlugin({
      NODE_ENV: 'development'
    })
  ]
})