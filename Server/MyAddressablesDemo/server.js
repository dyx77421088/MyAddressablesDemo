// // const throttle = require('express-throttle');
// // const express = require("express")
// // const bytes = require('bytes');
// // const path = require('path');

// // const app = express()
// // const port = 3000



// // app.use(throttle({
// //   "rate": "1000/sec", // 每秒钟允许传输的字节数
// //   "chunkSize": 1024, // 数据块大小
// //   "minDelay": 0 // 最小延迟时间
// // }));

// // app.use(express.static('public')); // 替换成你自己的静态文件目录路径

// // app.listen(port, ()=> {
// //     console.log(`server runing port=${port}`);
// // })

// const express = require('express');
// const throttle = require('express-throttle');

// const app = express();

// // 限制下载速度为每秒钟 100KB
// app.use(throttle({
//   "burst": 10,
//   "rate": "1/s",
//   "on_throttle": function(req, res, rate) {
//     console.log(rate);
//     res.set("X-Throttle", rate);
//   }
// }));
// // 静态文件目录
// app.use(express.static('public'));


// // 处理请求
// app.get('/', (req, res) => {
//   res.send('Hello World!');
// });

// app.listen(3000, () => {
//   console.log('Server started on port 3000');
// });



// const express = require('express');
// const slowDown = require('express-slow-down');
// const path = require('path');

// const app = express();

// // 设置每秒最大下载速度为 100KB
// const speedLimit = slowDown({
//   windowMs: 1000, // 时间窗口为1秒
//   delayAfter: 1, // 在第2次请求之后开始添加延迟
//   delayMs: 500, // 延迟500毫秒
// });

// // 配置静态资源目录
// app.use('/', express.static(path.join(__dirname, 'public')));

// // 将中间件添加到 /static 路由上，以限制下载速度
// app.use('/gameobject', speedLimit);

// app.use(speedLimit)

// app.listen(3000, function(){
//   console.log('Server started on port 3000');
// });



// const express = require('express');
// const rateLimiter = require('rate-limiter-flexible');
// const path = require('path');

// const app = express();

// const opts = {
//   points: 2, // 每秒钟允许的最大请求数
//   duration: 1, // 时间窗口为1秒
// };

// const rateLimiterMiddleware = new rateLimiter.RateLimiterMemory(opts);

// // 配置静态资源目录
// app.use('/static', express.static(path.join(__dirname, 'public')));

// // 将中间件添加到 /static 路由上，以限制下载速度
// app.use('/static', (req, res, next) => {
//   rateLimiterMiddleware.consume(req.ip, 1)
//     .then(() => {
//       next();
//     })
//     .catch((rejRes) => {
//       res.status(429).send('Too Many Requests');
//     });
// });

// app.listen(3000, function(){
//   console.log('Server started on port 3000');
// });

// const express = require('express');
// const rateLimit = require('express-rate-limit');
// const slowDown = require('express-slow-down');

// const app = express();

// // 设置每分钟最多只能发起 100 个请求
// const limiter = rateLimit({
//   windowMs: 60 * 1000, // 1 分钟
//   max: 100,
// });

// // 设置下载速度为 100kb/s
// const speedLimiter = slowDown({
//   windowMs: 30 * 1000, // 每 30 秒计算一次下载速度
//   delayAfter: 1, // 超过 1 个请求后开始限制下载速度
//   delayMs: 1000 / 100, // 每个请求之间至少间隔 1s
// });

// app.use('/', limiter, speedLimiter, express.static('public'));

// app.listen(3000, () => {
//   console.log('Server started on port 3000');
// });











const express = require('express');
const { createReadStream } = require('fs');
const { Transform } = require('stream');

const app = express();
var t = 0;
// 定义一个转换流，用于限制下载速度
class ThrottleTransform extends Transform {
  constructor(options) {
    super(options);
    this.rate = options.rate || 1024; // 默认 1KB/s
    this.timer = null;
    this.totalBytes = 0;
  }

  _transform(chunk, encoding, callback) {
    this.totalBytes += chunk.length;

    const throttledChunk = Buffer.alloc(chunk.length);
    for (let i = 0; i < chunk.length; i++) {
      throttledChunk[i] = chunk[i];
    }
    const delay = Math.ceil(throttledChunk.length / this.rate * 1000);
    // console.log(delay);
    if (this.timer) {
      clearTimeout(this.timer);
    }

    this.timer = setTimeout(() => {
      callback(null, throttledChunk);
    }, 1);
    // callback(null, throttledChunk);
  }

  _final(callback) {
    console.log(`Sent ${this.totalBytes} bytes`);
    callback();
  }
}

// 静态文件路由
// app.use('/', express.static(__dirname + '/public', {
//   setHeaders: (res, path) => {
//     res.setHeader('Content-Disposition', 'attachment');
//     res.setHeader('Content-Type', 'application/octet-stream');
//     res.setHeader('Content-Transfer-Encoding', 'binary');
//   }
// }));

// 自定义路由，使用转换流来限制下载速度
app.get('/:filename', (req, res, next) => {
  try {
    const { filename } = req.params;
    console.log("文件是" + filename);
    const fileStream = createReadStream(__dirname + '/public/' + filename);
    fileStream.on('error', next);
    const throttleTransform = new ThrottleTransform({ rate: 2 * 1024 * 1024 });

    res.attachment(filename);

    fileStream.pipe(throttleTransform).pipe(res);
    }
    catch(e) {

    }
});

app.get('/gameobject/:filename', (req, res, next) => {
    const { filename } = req.params;
  
    try {
        const fileStream = createReadStream(__dirname + '/public/gameobject/' + filename);
        fileStream.on('error', next);
        const throttleTransform = new ThrottleTransform({ rate: 20 * 1024 * 1024 });
      
        res.attachment(filename);
      
        fileStream.pipe(throttleTransform).pipe(res);
    }
    catch (e) {
        res.status(500).send('Internal Server Error');
    }
    
  });
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).send('Internal Server Error');
});
app.listen(3000, () => console.log('Server started on port 3000'));


// const express = require('express');
// const fs = require('fs');

// const app = express();

// app.get('/', (req, res, next) => {
//   const stream = fs.createReadStream('/path/to/file');
//   stream.on('error', next);
//   stream.pipe(res);
// });

// app.use((err, req, res, next) => {
//   console.error(err.stack);
//   res.status(500).send('Internal Server Error');
// });

// app.listen(3000, () => {
//   console.log('Server listening on port 3000.');
// });







