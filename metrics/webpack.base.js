const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const VueLoaderPlugin = require("vue-loader/lib/plugin");

module.exports = {
  entry: {
    main: path.resolve(__dirname, "./Content/index.ts"),
    styles: path.resolve(__dirname, "./Content/styles/styles.scss")
  },
  output: {
    path: path.resolve(__dirname, "./wwwroot"),
    publicPath: ".",
    filename: "./js/[name].js"
  },
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: "vue-loader"
      },
      {
        test: /\.scss$/,
        use: [
          process.env.NODE_ENV !== "production"
            ? "style-loader"
            : MiniCssExtractPlugin.loader,
          "css-loader",
          "postcss-loader",
          "sass-loader"
        ]
      },
      {
        test: /\.(png|jpe?g|gif|svg)$/,
        loader: "url-loader",
        query: { limit: 4000, name: "./img/[name].[ext]" }
      },
      {
        test: /\.(woff2?|eot|ttf|itf)(\?.*)?$/,
        loader: "url-loader",
        query: { limit: 4000, name: "./fonts/[name].[ext]" }
      },
      {
        test: /\.tsx?$/,
        loader: "ts-loader",
        exclude: /node_modules/,
        options: {
          appendTsSuffixTo: [/\.vue$/]
        }
      }
    ]
  },
  resolve: {
    extensions: [".ts", ".js", ".vue", ".json"],
    alias: {
      vue$: "vue/dist/vue.esm.js"
    }
  },
  plugins: [new VueLoaderPlugin()]
};
