const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const merge = require('webpack-merge');
const base = require('./webpack.base');
const uglifyJs = require('uglifyjs-webpack-plugin')
const purgecss = require('purgecss-webpack-plugin')

module.exports = merge(base, {
  optimization: {
    minimizer: [new uglifyJs({sourceMap: true})]
  },
  mode: 'production',
  devtool: 'source-map',
  plugins: [
    new MiniCssExtractPlugin({
      filename: './css/[name].css'
    }),
    new purgecss({
      content: ['./Views/**/*.cshtml', './Content/Components/**/*.vue'],
      css: ['./wwwroot/css/**/*.css']
    })
  ]
});