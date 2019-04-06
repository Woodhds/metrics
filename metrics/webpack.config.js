const merge = require('webpack-merge');
module.exports = merge(require(`./webpack.${process.env.NODE_ENV}`), {
})