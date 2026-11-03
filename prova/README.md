# Extração de contorno

Com Mono instalado, execute os comandos a seguir na raiz do repositório:

```sh
mcs -unsafe -r:System.Drawing -out:prova/contorno.exe prova/contorno.cs
mono prova/contorno.exe
```

Sem argumentos, o programa lê `Imagempb.png` deste diretório e grava
`Imagempb_contorno.png`. Também é possível informar caminhos para a entrada e
a saída:

```sh
mono prova/contorno.exe prova/Imagempb.png prova/resultado.png
```
