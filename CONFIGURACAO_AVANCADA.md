# ⚙️ Configuração Avançada - Revit AI Assistant

## 🔧 Personalização do Sistema

### Modificar Prompt do Sistema

Para personalizar as respostas da IA, edite o arquivo `Services/TogetherAIService.cs`:

```csharp
var defaultSystemPrompt = @"Você é um assistente especializado em Revit para engenheiros brasileiros.
Responda sempre em português, com foco em:
- Normas técnicas brasileiras (ABNT)
- Práticas locais de construção
- Código de obras municipais
- Padrões BIM nacionais

Forneça exemplos práticos e considere o contexto brasileiro.";
```

### Configurações de API

Edite `app.config` para ajustar parâmetros:

```xml
<appSettings>
  <!-- Modelo de IA (não altere se não souber) -->
  <add key="TogetherAI_Model" value="meta-llama/Llama-3.3-70B-Instruct-Turbo" />
  
  <!-- URL da API (não altere) -->
  <add key="TogetherAI_BaseUrl" value="https://api.together.xyz/v1/chat/completions" />
  
  <!-- Configurações de timeout (em segundos) -->
  <add key="ApiTimeout" value="30" />
  
  <!-- Tamanho máximo do contexto -->
  <add key="MaxContextLength" value="8000" />
</appSettings>
```

## 🎨 Personalização da Interface

### Cores e Tema

Edite `UI/AIAssistantWindow.xaml` para alterar cores:

```xml
<!-- Cor principal (verde) -->
<Setter Property="Background" Value="#FF2E7D32"/>

<!-- Cor secundária (azul) -->
<Setter Property="Background" Value="#FF1976D2"/>

<!-- Cores personalizadas -->
<Setter Property="Background" Value="#FF6A1B9A"/> <!-- Roxo -->
<Setter Property="Background" Value="#FFD32F2F"/> <!-- Vermelho -->
<Setter Property="Background" Value="#FF0288D1"/> <!-- Azul claro -->
```

### Tamanho da Janela

```xml
<Window Title="Revit AI Assistant" 
        Height="700" Width="900"  <!-- Aumentar tamanho -->
        MinHeight="500" MinWidth="700"> <!-- Tamanho mínimo -->
```

### Fontes e Texto

```xml
<!-- Tamanho da fonte do chat -->
<TextBlock FontSize="16"  <!-- Padrão: 14 -->
           FontFamily="Segoe UI" <!-- Fonte personalizada -->
           TextWrapping="Wrap"/>
```

## 🔐 Configurações de Segurança

### Proxy Corporativo

Se sua empresa usa proxy, adicione em `Services/TogetherAIService.cs`:

```csharp
private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler()
{
    Proxy = new WebProxy("http://proxy.empresa.com:8080")
    {
        UseDefaultCredentials = true
    }
});
```

### Certificados SSL

Para ambientes com certificados corporativos:

```csharp
ServicePointManager.ServerCertificateValidationCallback = 
    (sender, certificate, chain, sslPolicyErrors) => true;
```

**⚠️ Atenção**: Use apenas em ambientes controlados!

## 📊 Logs e Diagnóstico

### Habilitando Logs Detalhados

Adicione em `app.config`:

```xml
<system.diagnostics>
  <trace autoflush="true" indentsize="4">
    <listeners>
      <add name="fileListener" 
           type="System.Diagnostics.TextWriterTraceListener" 
           initializeData="RevitAI.log" />
    </listeners>
  </trace>
</system.diagnostics>
```

No código, adicione logs:

```csharp
using System.Diagnostics;

// Log de debug
Trace.WriteLine($"Enviando mensagem: {userMessage}");

// Log de erro
Trace.WriteLine($"Erro na API: {ex.Message}");
```

### Monitoramento de Performance

Adicione métricas de tempo:

```csharp
var stopwatch = Stopwatch.StartNew();
var response = await _aiService.SendMessageAsync(messages);
stopwatch.Stop();

StatusTextBlock.Text = $"Resposta em {stopwatch.ElapsedMilliseconds}ms";
```

## 🌐 Configurações de Rede

### Timeout Personalizado

```csharp
_httpClient.Timeout = TimeSpan.FromSeconds(60); // 60 segundos
```

### Retry Logic

Implementar tentativas automáticas:

```csharp
public async Task<string> SendMessageWithRetryAsync(List<ChatMessage> messages, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await SendMessageAsync(messages);
        }
        catch (HttpRequestException) when (i < maxRetries - 1)
        {
            await Task.Delay(2000 * (i + 1)); // Backoff exponencial
        }
    }
    throw new Exception("Falha após múltiplas tentativas");
}
```

## 🎯 Contexto Personalizado

### Informações Específicas da Empresa

Edite `Utils/RevitHelper.cs` para incluir dados específicos:

```csharp
public static string GetCompanyContext(Document doc)
{
    var context = new StringBuilder();
    context.AppendLine("=== CONTEXTO DA EMPRESA ===");
    
    // Template da empresa
    if (doc.PathName.Contains("TEMPLATE_EMPRESA"))
    {
        context.AppendLine("Template: Padrão da Empresa XYZ");
    }
    
    // Padrões específicos
    var projectInfo = doc.ProjectInformation;
    if (projectInfo != null)
    {
        // Buscar parâmetros específicos da empresa
        foreach (Parameter param in projectInfo.Parameters)
        {
            if (param.Definition.Name.StartsWith("EMP_"))
            {
                context.AppendLine($"{param.Definition.Name}: {param.AsString()}");
            }
        }
    }
    
    return context.ToString();
}
```

### Filtros de Contexto

Controlar que informações enviar:

```csharp
public class ContextFilter
{
    public bool IncludeSelection { get; set; } = true;
    public bool IncludeViews { get; set; } = true;
    public bool IncludePhases { get; set; } = false;
    public bool IncludeWorksets { get; set; } = false;
    public bool IncludeProjectInfo { get; set; } = true;
}
```

## 🚀 Performance e Otimização

### Cache de Respostas

Implementar cache local:

```csharp
private static readonly Dictionary<string, string> _responseCache = 
    new Dictionary<string, string>();

public async Task<string> SendMessageWithCacheAsync(string message)
{
    var hash = message.GetHashCode().ToString();
    
    if (_responseCache.ContainsKey(hash))
    {
        return _responseCache[hash];
    }
    
    var response = await SendMessageAsync(message);
    _responseCache[hash] = response;
    
    return response;
}
```

### Limitação de Histórico

```csharp
private const int MAX_HISTORY = 20;

private void AddMessageToHistory(ChatMessage message)
{
    _chatHistory.Add(message);
    
    if (_chatHistory.Count > MAX_HISTORY)
    {
        _chatHistory.RemoveAt(0);
    }
}
```

## 🔄 Integração com Outros Sistemas

### Conexão com Banco de Dados

```csharp
public class DatabaseContext
{
    private readonly string _connectionString;
    
    public void SaveChatHistory(List<ChatMessage> messages)
    {
        // Salvar histórico no banco
    }
    
    public List<ChatMessage> LoadChatHistory(string userId)
    {
        // Carregar histórico do banco
        return new List<ChatMessage>();
    }
}
```

### API REST Personalizada

```csharp
public class CustomAPIService
{
    public async Task<string> SendToCustomAPI(string message, string context)
    {
        var payload = new
        {
            message = message,
            context = context,
            userId = Environment.UserName,
            timestamp = DateTime.UtcNow
        };
        
        // Enviar para API interna da empresa
        return await PostToInternalAPI(payload);
    }
}
```

## 📱 Configurações de Usuário

### Preferências Persistentes

Estender `Properties/Settings.settings`:

```xml
<Settings>
  <Setting Name="PreferredLanguage" Type="System.String" Scope="User">
    <Value Profile="(Default)">pt-BR</Value>
  </Setting>
  <Setting Name="ChatFontSize" Type="System.Int32" Scope="User">
    <Value Profile="(Default)">14</Value>
  </Setting>
  <Setting Name="AutoSaveHistory" Type="System.Boolean" Scope="User">
    <Value Profile="(Default)">True</Value>
  </Setting>
</Settings>
```

### Interface de Configurações

Criar janela de configurações:

```csharp
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        LoadSettings();
    }
    
    private void LoadSettings()
    {
        FontSizeSlider.Value = Properties.Settings.Default.ChatFontSize;
        AutoSaveCheckBox.IsChecked = Properties.Settings.Default.AutoSaveHistory;
    }
    
    private void SaveSettings()
    {
        Properties.Settings.Default.ChatFontSize = (int)FontSizeSlider.Value;
        Properties.Settings.Default.AutoSaveHistory = AutoSaveCheckBox.IsChecked ?? false;
        Properties.Settings.Default.Save();
    }
}
```

## 🧪 Testes e Debug

### Modo Debug

Adicionar switch de debug:

```csharp
#if DEBUG
    private void AddDebugInfo(string info)
    {
        DebugTextBlock.Text += $"[DEBUG] {DateTime.Now:HH:mm:ss} - {info}\n";
    }
#endif
```

### Simulação de API

Para desenvolvimento sem API:

```csharp
public class MockAIService : ITogetherAIService
{
    public async Task<string> SendMessageAsync(List<ChatMessage> messages)
    {
        await Task.Delay(1000); // Simular delay
        
        var responses = new[]
        {
            "Esta é uma resposta simulada para desenvolvimento.",
            "Para criar uma família, vá em File > New > Family...",
            "O problema pode estar relacionado à configuração do workset."
        };
        
        return responses[new Random().Next(responses.Length)];
    }
}
```

## 📚 Documentação Interna

### Comentários de Código

```csharp
/// <summary>
/// Serviço principal para comunicação com a API Together.ai
/// Gerencia autenticação, cache e retry logic
/// </summary>
/// <remarks>
/// Este serviço implementa o padrão Singleton para reutilizar conexões HTTP
/// e manter cache de respostas para melhor performance
/// </remarks>
public class TogetherAIService
{
    // Implementação...
}
```

### README Técnico

Criar `TECHNICAL_README.md` com:
- Arquitetura do plugin
- Fluxo de dados
- Pontos de extensão
- Troubleshooting técnico

---

**⚡ Essas configurações são para usuários avançados. Faça backup antes de modificar!**