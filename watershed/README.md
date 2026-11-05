# Watershed

Execute na pasta raiz do repositorio:

```powershell
dotnet run --project watershed/watershed.csproj
```

Por padrao, o programa processa `cells.png`. Para usar outra imagem:

```powershell
dotnet run --project watershed/watershed.csproj -- caminho\da\imagem.png
```

O programa imprime no terminal a quantidade de sementes encontradas, usada
como estimativa da quantidade de celulas.
