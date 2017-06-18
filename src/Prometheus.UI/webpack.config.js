var path = require('path');
var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
  entry: {
    'main': './src/main.ts'
  },

  output: {
    path: path.resolve(__dirname, 'wwwroot'),
    publicPath: '/',
    filename: '[name].[hash].js',
    chunkFilename: '[id].chunk.js'
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js']
  },

  module: {
    rules: [{
        test: /\.ts$/,
        use: ['awesome-typescript-loader?silent=true', 'angular2-template-loader']
      },
      {
        test: /\.css$/,
        include: path.resolve(__dirname, 'src', 'app'),
        use: 'raw-loader'
      },
      {
        test: /\.scss$/,
        include: /src/,
        use: ExtractTextPlugin.extract({
          use: ['css-loader', 'sass-loader'],
          fallback: 'style-loader'
        })
      },
      {
        test: /\.html$/,
        use: 'html-loader'
      },
      {
        test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
        use: 'file-loader?name=assets/[name].[hash].[ext]'
      },
      {
        test: require.resolve("pace-js"),
        loader: "imports-loader?define=>false"
      }
    ]
  },

  plugins: [
    new webpack.ContextReplacementPlugin(/angular(\\|\/)core(\\|\/)@angular/, path.join(__dirname, './Client')),

    new ExtractTextPlugin('[name].[hash].css'),

    new webpack.ProvidePlugin({
      jQuery: 'jquery',
      $: 'jquery',
      jquery: 'jquery'
    }),

    new HtmlWebpackPlugin({
      template: 'src/index.html',
      favicon: 'src/favicon.ico',
      xhtml: true,
      hash: true
    }),

    new CopyWebpackPlugin([
      { from: 'src/error.html' }
    ])
  ],


  devServer: {
    port: 8080,
    historyApiFallback: {
      index: '/'
    },
    stats: 'minimal'
  }
};
