
class TailwindExtractor {
  static extract(content) {
    return content.match(/[\w-/:]+(?<!:)/g) || []
  }
}

const purgecss = require('@fullhuman/postcss-purgecss')({
  content: [
    './Content/Components/**/*.vue',
    './views/**/*.cshtml'
  ],
  extractors: [{
    extractor: TailwindExtractor,
    extensions: ['html', 'js', 'cshtml', 'vue']
  }]
})


module.exports = {
 
  plugins: [
    require('tailwindcss')('./Content/tailwind.config.js'),
    require('autoprefixer'),
    ...process.env.NODE_ENV === 'production'
      ? [purgecss]
      : []
  ]
};