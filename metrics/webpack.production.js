const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const merge = require('webpack-merge');
const base = require('./webpack.base');
const terserJs = require('terser-webpack-plugin');
const purgecss = require('purgecss-webpack-plugin');
const path = require('path');

class TailwindExtractor {
	static extract(content) {
		return content.match(/[A-Za-z0-9-_:\/]+/g) || []
	}
}

module.exports = merge(base, {
  mode: 'production',
  optimization: {
    minimizer: [new terserJs({sourceMap: true})]
  },
  devtool: 'source-map',
  plugins: [
    new MiniCssExtractPlugin({
      filename: './css/[name].css'
    }),
    new purgecss({
      paths: 
        [
          path.join(__dirname, '/Content/Components/**/*.vue')
        ],
      extractors: [
        {
          extractor: TailwindExtractor,
          extensions: ['html', 'js', 'cshtml', 'vue']
        }
      ]
    })
  ]
});