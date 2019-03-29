module.exports = {
  devTool: 'eval-sourcemap',
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
};