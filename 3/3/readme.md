# Equalização de histograma

## Requisitos

- Windows
- .NET 10 SDK

## Como executar

Na raiz do repositório, execute:

```powershell
dotnet run --project .\3\3\EqualizacaoHistograma.csproj -- .\3\3\imagem.jpg
```

O programa salva `imagem_equalizada.png` na mesma pasta da imagem de entrada.

Para escolher outro arquivo de saída, informe-o como segundo argumento:

```powershell
dotnet run --project .\3\3\EqualizacaoHistograma.csproj -- .\3\2\imagem.jpg .\3\3\resultado.png
```
