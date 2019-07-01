const merge = require("webpack-merge");
const webpack = require("webpack");
const dotenv = require("dotenv").config({ path: __dirname + "/.env" });

module.exports = merge(require(`./webpack.${process.env.NODE_ENV}`), {
  plugins: [
    new webpack.EnvironmentPlugin({
      NODE_ENV: 'production'
    })
  ]
});
