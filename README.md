# 🤖 Revit AI Assistant - Plugin para Revit 2026

**Um assistente de IA inteligente para Autodesk Revit 2026, desenvolvido com integração à API Together.ai e modelo Meta Llama 3.3 70B Instruct Turbo gratuito.**

![Revit AI Assistant](https://img.shields.io/badge/Revit-2026-blue) ![Together.ai](https://img.shields.io/badge/Together.ai-Meta%20Llama%203.3%2070B-green) ![License](https://img.shields.io/badge/License-Free-brightgreen)

## 🌟 Características Principais

- **🧠 IA Avançada**: Utiliza o modelo Meta Llama 3.3 70B Instruct Turbo da Together.ai
- **💰 Totalmente Gratuito**: Sem custos de uso ou limitações
- **🔍 Consciente do Contexto**: Entende automaticamente o estado atual do seu projeto Revit
- **💬 Interface de Chat Moderna**: Interface amigável com chat em tempo real
- **🛠️ Suporte Especializado**: Focado em modelagem BIM, famílias, parâmetros e API do Revit
- **⚡ Respostas Rápidas**: Processamento serverless para respostas instantâneas

## 🎯 O que o Assistente pode Fazer

### Modelagem e Design
- Técnicas de modelagem avançadas no Revit
- Melhores práticas para criação de famílias
- Gerenciamento de parâmetros e fórmulas
- Organização de projetos e workflows
- Padrões BIM e coordenação

### Resolução de Problemas
- Diagnóstico de problemas comuns no Revit
- Soluções para erros e warnings
- Otimização de performance
- Resolução de conflitos de workset

### Desenvolvimento
- Orientação sobre API do Revit
- Desenvolvimento de plugins e add-ins
- Automação de tarefas com Dynamo
- Scripting e programação em C#

## 🚀 Instalação

### Pré-requisitos
- Autodesk Revit 2026
- .NET Framework 4.8 ou superior
- Conexão com internet
- Conta gratuita na Together.ai

### Passos de Instalação

1. **Obtenha uma chave de API gratuita da Together.ai:**
   - Visite [https://api.together.xyz/](https://api.together.xyz/)
   - Crie uma conta gratuita (não é necessário cartão de crédito)
   - Copie sua chave de API

2. **Compile o Plugin:**
   ```bash
   # Clone ou baixe o projeto
   cd RevitAIAssistant
   
   # Compile usando Visual Studio ou MSBuild
   msbuild RevitAIAssistant.sln /p:Configuration=Release /p:Platform=x64
   ```

3. **Instale o Plugin:**
   - Copie os arquivos compilados para:
     ```
     %APPDATA%\Autodesk\Revit\Addins\2026\
     ```
   - Ou use o instalador incluído no projeto

4. **Configure a API:**
   - Abra o Revit 2026
   - Procure pela aba "AI Assistant" na ribbon
   - Clique em "AI Assistant" 
   - Digite sua chave de API quando solicitado

## 📖 Como Usar

### Interface Principal

O plugin adiciona uma nova aba "AI Assistant" ao Revit com dois botões:
- **🤖 AI Assistant**: Abre a janela principal do chat
- **❓ Help**: Mostra instruções e ajuda

### Recursos da Interface

1. **Área de Chat**: Interface de conversação moderna com histórico
2. **Contexto do Revit**: Mostra automaticamente informações do projeto atual
3. **Controles**:
   - Checkbox "Include Revit context" para incluir contexto nas mensagens
   - Botão "Clear" para limpar o histórico
   - Botão "Settings" para reconfigurar a API

### Exemplos de Perguntas

```
👤 "Como criar uma família paramétrica de janela?"
🤖 [Resposta detalhada com passos específicos]

👤 "Por que meu modelo está lento?"
🤖 [Análise de performance com sugestões de otimização]

👤 "Como usar a API do Revit para criar elementos automaticamente?"
🤖 [Exemplos de código C# com explicações]
```

### Atalhos de Teclado

- **Ctrl + Enter**: Enviar mensagem
- **Enter**: Nova linha no campo de texto

## 🔧 Desenvolvimento

### Estrutura do Projeto

```
RevitAIAssistant/
├── Commands/              # Comandos do Revit
│   ├── AIAssistantCommand.cs
│   └── HelpCommand.cs
├── Services/              # Serviços de integração
│   └── TogetherAIService.cs
├── Models/                # Modelos de dados
│   ├── ChatMessage.cs
│   ├── TogetherAIRequest.cs
│   └── TogetherAIResponse.cs
├── UI/                    # Interface do usuário
│   ├── AIAssistantWindow.xaml
│   └── AIAssistantWindow.xaml.cs
├── Utils/                 # Utilitários
│   └── RevitHelper.cs
├── Properties/            # Configurações
│   ├── Settings.settings
│   └── AssemblyInfo.cs
├── Application.cs         # Aplicação principal
└── RevitAIAssistant.addin # Manifesto do plugin
```

### Tecnologias Utilizadas

- **C# .NET Framework 4.8**: Linguagem principal
- **WPF**: Interface de usuário
- **Revit API 2026**: Integração com Revit
- **Newtonsoft.Json**: Processamento JSON
- **Together.ai API**: Serviço de IA

### Personalizações

O plugin pode ser personalizado editando:

1. **Prompt do Sistema** (`TogetherAIService.cs`):
   ```csharp
   var defaultSystemPrompt = @"Seu prompt personalizado aqui...";
   ```

2. **Configurações de API** (`app.config`):
   ```xml
   <add key="TogetherAI_Model" value="meta-llama/Llama-3.3-70B-Instruct-Turbo" />
   ```

3. **Interface** (`AIAssistantWindow.xaml`):
   - Cores, layout, estilos

## 🔐 Segurança e Privacidade

- **Chave de API**: Armazenada localmente de forma segura
- **Dados do Projeto**: Apenas contexto básico é enviado (nunca geometria completa)
- **Comunicação**: HTTPS criptografado com Together.ai
- **Sem Telemetria**: Nenhum dado é coletado pelos desenvolvedores

## 🐛 Solução de Problemas

### Problemas Comuns

1. **"Erro de API Key"**:
   - Verifique se a chave está correta
   - Teste a chave no site da Together.ai

2. **"Plugin não aparece no Revit"**:
   - Verifique se os arquivos estão na pasta correta
   - Confirme que o Revit 2026 está sendo usado
   - Verifique o arquivo .addin

3. **"Erro de conexão"**:
   - Verifique a conexão com internet
   - Confirme que firewalls não estão bloqueando

### Logs e Diagnóstico

Os logs do plugin podem ser encontrados em:
```
%LOCALAPPDATA%\Autodesk\Revit\Autodesk Revit 2026\Journals\
```

## 📄 Licença

Este projeto é de código aberto e gratuito para uso pessoal e comercial.

## 🤝 Contribuições

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Faça um pull request

## 📞 Suporte

- **Issues**: Use o sistema de issues do GitHub
- **Documentação**: Consulte este README
- **Together.ai**: [Documentação da API](https://docs.together.ai/)

## 🎉 Agradecimentos

- **Together.ai**: Pelo modelo gratuito Meta Llama 3.3 70B
- **Autodesk**: Pelo Revit API
- **Comunidade BIM**: Pelo feedback e sugestões

---

**Desenvolvido com ❤️ para a comunidade BIM brasileira**

*Transforme sua experiência no Revit com o poder da Inteligência Artificial!*