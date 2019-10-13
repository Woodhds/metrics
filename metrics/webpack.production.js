const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const merge = require('webpack-merge');
const base = require('./webpack.base');
const terserJs = require('terser-webpack-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');

module.exports = merge(base, {
  mode: 'production',
  optimization: {
    minimizer: [new terserJs({
      sourceMap: true
    }), new OptimizeCSSAssetsPlugin({})]
  },
  devtool: '@eval-source-map',
  plugins: [
    new MiniCssExtractPlugin({
      filename: './css/[name].css'
    })
  ]
});