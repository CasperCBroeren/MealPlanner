const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const SWPrecacheWebpackPlugin = require('sw-precache-webpack-plugin');
const WebpackPwaManifest = require('webpack-pwa-manifest');


const bundleOutputDir = './wwwroot/dist'; 

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);

    return [{
        stats: { modules: false },
        entry: { 'main': './ClientApp/boot-app.js' },
        resolve: {
            extensions: ['.js', '.vue'],
            alias: {
                'vue$': 'vue/dist/vue',
                'components': path.resolve(__dirname, './ClientApp/components'),
                'views': path.resolve(__dirname, './ClientApp/views'),
                'utils': path.resolve(__dirname, './ClientApp/utils'),
                'api': path.resolve(__dirname, './ClientApp/store/api')
            }
        },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            publicPath: '/dist/'
        },
        module: {
            rules: [
                { test: /\.vue$/, include: /ClientApp/, use: 'vue-loader' },
                { test: /\.js$/, include: /ClientApp/, use: 'babel-loader' },
                { test: /\.css$/, use:   ['style-loader', 'css-loader']  },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' },
                  {
                    test: /.(ttf|otf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
                    use: [{
                        loader: 'file-loader',
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'fonts/',    // where the fonts will go
                            publicPath: '../'       // override the default path
                        }
                    }]
                },
            ]
        },
        plugins: [
            new SWPrecacheWebpackPlugin(
                {
                    cacheId: 'mealplanner',
                    dontCacheBustUrlsMatching: /\.\w{8}\./,
                    filename: '../service-worker.js',
                    minify: false,
                    dynamicUrlToDependencies: {
                        '/': ['./wwwroot/plain.html']
                    },
                    navigateFallback: '/',
                    staticFileGlobsIgnorePatterns: [/\.map$/, /manifest\.json$/]
                }
            ),
            new WebpackPwaManifest({
                filename: "manifest.json",
                name: 'Maaltijdplanner',
                short_name: 'MP',
                description: 'Plan je maaltijd voor een week met gemak!',
                background_color: '#E5E5E5',
                theme_color: '#88B337', 
                start_url: './login',
                icons: [
                    {
                        src: path.resolve('./wwwroot/icon.png'),
                        sizes: [96, 128, 192, 256, 384, 512],
                        destination: path.join('assets', 'icons')
                    }
                ]
            }),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            }),
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: '"production"'
                }
            })
        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(bundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
                // Plugins that apply in production builds only
                new webpack.optimize.UglifyJsPlugin(),
                new ExtractTextPlugin('site.css')
            ])
    }];
};
