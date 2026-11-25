# Zhang-Suen

Para compilar e executar com Mono:

```sh
cd 4/2
mcs -unsafe -r:System.Drawing -out:ZhangSuen.exe ZhangSuen.cs
mono ZhangSuen.exe
```

Sem argumentos, o programa lê `Imagempb.png` deste diretório e grava
`Imagempb_zhangsuen.png`. Também é possível informar outros caminhos:

```sh
mono ZhangSuen.exe imagem.png resultado.png
```
