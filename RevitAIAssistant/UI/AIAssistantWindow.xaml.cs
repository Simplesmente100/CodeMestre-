using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Autodesk.Revit.UI;
using RevitAIAssistant.Models;
using RevitAIAssistant.Services;
using RevitAIAssistant.Utils;

namespace RevitAIAssistant.UI
{
    public partial class AIAssistantWindow : Window
    {
        private readonly UIDocument _uidoc;
        private TogetherAIService _aiService;
        private readonly List<ChatMessage> _chatHistory;
        private bool _isProcessing = false;

        public AIAssistantWindow(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            _chatHistory = new List<ChatMessage>();

            // Initialize AI service with API key from app settings or user input
            var apiKey = GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                ShowApiKeyDialog();
                return;
            }

            _aiService = new TogetherAIService(apiKey);
            
            // Initialize the UI
            InitializeUI();
        }

        private string GetApiKey()
        {
            // Try to get API key from user settings first
            try
            {
                return Properties.Settings.Default.TogetherAI_ApiKey;
            }
            catch
            {
                // Fallback to app.config
                try
                {
                    return ConfigurationManager.AppSettings["TogetherAI_ApiKey"];
                }
                catch
                {
                    return null;
                }
            }
        }

        private void ShowApiKeyDialog()
        {
            var dialog = new Window
            {
                Title = "Together.ai API Key Required",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var titleText = new TextBlock
            {
                Text = "Together.ai API Key Setup",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(20, 20, 20, 10)
            };
            Grid.SetRow(titleText, 0);
            grid.Children.Add(titleText);

            var instructionText = new TextBlock
            {
                Text = "To use the AI Assistant, you need a free API key from Together.ai:\n\n" +
                       "1. Visit https://api.together.xyz/\n" +
                       "2. Sign up for a free account\n" +
                       "3. Copy your API key\n" +
                       "4. Paste it below:",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20, 0, 20, 15)
            };
            Grid.SetRow(instructionText, 1);
            grid.Children.Add(instructionText);

            var apiKeyTextBox = new TextBox
            {
                Margin = new Thickness(20, 0, 20, 15),
                Padding = new Thickness(10),
                FontFamily = new FontFamily("Consolas"),
                Text = "Enter your Together.ai API key here..."
            };
            Grid.SetRow(apiKeyTextBox, 2);
            grid.Children.Add(apiKeyTextBox);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Margin = new Thickness(0, 0, 10, 0)
            };
            cancelButton.Click += (s, e) => { dialog.DialogResult = false; };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                IsDefault = true
            };
            okButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(apiKeyTextBox.Text) && 
                    !apiKeyTextBox.Text.Contains("Enter your"))
                {
                    Properties.Settings.Default.TogetherAI_ApiKey = apiKeyTextBox.Text.Trim();
                    Properties.Settings.Default.Save();
                    dialog.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Please enter a valid API key.", "Invalid API Key", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(okButton);
            Grid.SetRow(buttonPanel, 3);
            grid.Children.Add(buttonPanel);

            dialog.Content = grid;

            if (dialog.ShowDialog() == true)
            {
                var apiKey = Properties.Settings.Default.TogetherAI_ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _aiService = new TogetherAIService(apiKey);
                    InitializeUI();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void InitializeUI()
        {
            // Load current Revit context
            UpdateContextDisplay();

            // Set focus to message input
            MessageTextBox.Focus();
            MessageTextBox.SelectAll();

            // Initialize status
            StatusTextBlock.Text = "Ready - Connected to Together.ai";
        }

        private void UpdateContextDisplay()
        {
            try
            {
                var context = RevitHelper.GetRevitContext(_uidoc);
                ContextTextBlock.Text = context;
            }
            catch (Exception ex)
            {
                ContextTextBlock.Text = $"Error loading context: {ex.Message}";
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessageAsync();
        }

        private async void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                await SendMessageAsync();
            }
        }

        private async Task SendMessageAsync()
        {
            if (_isProcessing || string.IsNullOrWhiteSpace(MessageTextBox.Text) || 
                MessageTextBox.Text == "Type your question about Revit here...")
                return;

            var userMessage = MessageTextBox.Text.Trim();
            MessageTextBox.Clear();
            
            _isProcessing = true;
            UpdateUIForProcessing(true);

            try
            {
                // Add user message to chat
                var chatMessage = new ChatMessage(ChatRoles.User, userMessage);
                _chatHistory.Add(chatMessage);
                AddMessageToChat(chatMessage);

                // Prepare message for AI (include context if enabled)
                var messageForAI = userMessage;
                if (IncludeContextCheckBox.IsChecked == true)
                {
                    var context = RevitHelper.GetRevitContext(_uidoc);
                    messageForAI = $"Current Revit Context:\n{context}\n\nUser Question: {userMessage}";
                }

                // Get AI response
                StatusTextBlock.Text = "AI is thinking...";
                var response = await _aiService.SendMessageAsync(_chatHistory.Where(m => m.Role != ChatRoles.System).ToList());

                // Add AI response to chat
                var aiMessage = new ChatMessage(ChatRoles.Assistant, response);
                _chatHistory.Add(aiMessage);
                AddMessageToChat(aiMessage);

                StatusTextBlock.Text = "Ready";
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error: {ex.Message}";
                StatusTextBlock.Text = "Error occurred";
                
                var errorChatMessage = new ChatMessage(ChatRoles.Assistant, 
                    $"I apologize, but I encountered an error: {ex.Message}\n\nPlease check your internet connection and API key, then try again.");
                AddMessageToChat(errorChatMessage);
            }
            finally
            {
                _isProcessing = false;
                UpdateUIForProcessing(false);
                MessageTextBox.Focus();
            }
        }

        private void AddMessageToChat(ChatMessage message)
        {
            var border = new Border();
            var textBlock = new TextBlock
            {
                Text = message.Content,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };

            if (message.Role == ChatRoles.User)
            {
                border.Style = (Style)FindResource("ChatBubbleUser");
                textBlock.Foreground = Brushes.White;
            }
            else
            {
                border.Style = (Style)FindResource("ChatBubbleAssistant");
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x21));
            }

            border.Child = textBlock;
            ChatPanel.Children.Add(border);

            // Auto-scroll to bottom
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                ChatScrollViewer.ScrollToEnd();
            }));
        }

        private void UpdateUIForProcessing(bool isProcessing)
        {
            SendButton.IsEnabled = !isProcessing;
            MessageTextBox.IsEnabled = !isProcessing;
            
            if (isProcessing)
            {
                SendButton.Content = "⏳ Sending...";
            }
            else
            {
                SendButton.Content = "📤 Send";
            }
        }

        private void ClearChatButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the chat history?", 
                                       "Clear Chat", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _chatHistory.Clear();
                ChatPanel.Children.Clear();
                
                // Add welcome message back
                var welcomeBorder = new Border
                {
                    Style = (Style)FindResource("ChatBubbleAssistant"),
                    Margin = new Thickness(10, 10, 50, 5)
                };
                
                welcomeBorder.Child = new TextBlock
                {
                    Text = "Hello! I'm your AI assistant for Revit. I can help you with modeling techniques, troubleshooting, API questions, and more. How can I assist you today?",
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x21, 0x21, 0x21))
                };
                
                ChatPanel.Children.Add(welcomeBorder);
                StatusTextBlock.Text = "Chat cleared - Ready";
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowApiKeyDialog();
        }
    }
}