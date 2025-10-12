const express = require('express');
const path = require('path');

const app = express();
const PORT = 8080;

// 1. Sirve los archivos estáticos (html, css, js) desde el directorio actual
app.use(express.static(__dirname));

// 2. Middleware de fallback: para cualquier otra petición que no sea un archivo,
//    devuelve el index.html. Esto reemplaza a app.get('/*').
app.use((req, res) => {
  res.sendFile(path.resolve(__dirname, 'index.html'));
});

app.listen(PORT, () => {
  console.log(`Frontend server running on http://localhost:${PORT}`);
});
