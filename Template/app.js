const express = require('express');
const app = express();

// Configurar Express para servir archivos estáticos desde el directorio raíz
// app.use(express.static(__dirname + '/Template'));
app.use(cors());

// Ruta para la página principal
app.get('/', (req, res) => {
  res.sendFile(__dirname + '/home.html');
});

// Iniciar el servidor en el puerto 8080
app.listen(8080, () => {
  console.log('El servidor está escuchando en el puerto 8080');
});
