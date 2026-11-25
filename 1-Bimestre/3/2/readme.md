# Fatiamento de planos de bits

## Requisitos

- Windows
- .NET 10 SDK

## Como executar

Na raiz do repositório, execute:

```powershell
dotnet run --project .\3\2\FatiamentoPlanoBits.csproj -- .\3\2\imagem.jpg
```

O programa gera os oito planos de bits (`imagem_plano_0.png` a
`imagem_plano_7.png`) na mesma pasta da imagem informada.

Para processar outra imagem, substitua `.\3\2\imagem.jpg` pelo caminho dela.
