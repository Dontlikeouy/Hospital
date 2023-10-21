using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hospital.Templates
{

    public class SourceImage
    {
        public SourceImage(string FirstSourceImage, string SecondSourceImage, MouseButtonEventHandler mouseDown)
        {
            this.FirstSourceImage = FirstSourceImage;
            this.SecondSourceImage = SecondSourceImage;
            this.mouseDown=mouseDown;   
        }
        private string firstSourceImage;
        public string FirstSourceImage
        {
            get
            {
                return firstSourceImage;
            }
            set
            {
                firstSourceImage = @"pack://application:,,,/Hospital;component/" + value;
            }
        }
        private string secondSourceImage;
        public string SecondSourceImage
        {
            get
            {
                return secondSourceImage;
            }
            set
            {
                secondSourceImage = @"pack://application:,,,/Hospital;component/" + value;
            }
        }
        public MouseButtonEventHandler mouseDown { get; set; }

    }

    public class MyButton
    {
        public string Text { get; set; }
        public MouseButtonEventHandler mouseDown { get; set; }
        public MyButton(string Text, MouseButtonEventHandler mouseDown)
        {
            this.Text = Text;
            this.mouseDown = mouseDown;
        }
        public TextBlock textBlock { get; set; }


    }
    public class MyTextBlock : MyButton
    {
        public MyTextBlock(string Text, MouseButtonEventHandler mouseDown, bool Select = false) : base(Text, mouseDown)
        {
            this.Text = Text;
            this.mouseDown = mouseDown;
            this.Select = Select;
        }
        public bool Select { get; set; } = false;

    }

    public class MyTextBox
    {
        public MyTextBox(int MaxLength = 128,bool skip=false, bool needSymbol = true)
        {
            this.MaxLength = MaxLength;
            this.needSymbol = needSymbol;
            this.skip = skip;
        }
        public MyTextBox(string text, bool skip = false, bool needSymbol = true)
        {
            this.text = text;
            this.needSymbol = needSymbol;
            this.skip = skip;
        }
        public int MaxLength { get; set; }
        public TextBox textBox { get; set; }
        public string text { get; set; }
        public bool needSymbol { get; set; }
        public bool skip { get; set; }

    }



    public class MyTemplate
    {
        

        static public ContentControl PopUps { get; set; }
        static public ContentControl PopUpsWarning { get; set; }


        static public void ClosePopUps_Click(object sender, MouseButtonEventArgs e)
        {
            PopUps.Content = null;
        }
        static public void ClosePopUpsWarning_Click(object sender, MouseButtonEventArgs e)
        {
            PopUpsWarning.Content = null;
        }
        static public Dictionary<string, MyTextBox> contentPopUps { get; set; }
        static public Dictionary<object, List<object>> contentMain { get; set; }



        static public Grid PopUps_Output(string Title, string TextContent, MyButton EndButton, bool closePopUps = false)
        {

            ControlTemplate template = new ControlTemplate(typeof(TextBox)); ;

            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));

            FrameworkElementFactory element = new FrameworkElementFactory(typeof(Border));
            element.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            element.SetValue(Border.BorderBrushProperty, (Brush)(new BrushConverter()).ConvertFrom("#3A7597"));
            element.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));
            element.SetValue(Border.PaddingProperty, new Thickness(2));
            gridFactory.AppendChild(element);

            FrameworkElementFactory element1 = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
            element1.SetValue(ScrollViewer.MarginProperty, new Thickness(2));
            gridFactory.AppendChild(element1);
            template.VisualTree = gridFactory;





            StackPanel stack;
            Border border;
            Grid Setgird;
            Grid gridPanel = new Grid
            {

                Children =
                {
                    new Grid
                    {
                        Background = (Brush)(new BrushConverter()).ConvertFrom("#F5F5F5"),
                        Opacity = 0.5
                    },
                    new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        MaxWidth=400,
                        MinWidth=200,
                        Background= (Brush)(new BrushConverter()).ConvertFrom("#fff"),
                        Children=
                        {
                            new Border
                            {
                                Background=(Brush)(new BrushConverter()).ConvertFrom("#3A7597"),
                                CornerRadius=new CornerRadius(5,5,0,0),
                                Child=Setgird=new Grid
                                {
                                    Margin=new Thickness(10),
                                    ColumnDefinitions=
                                    {
                                        new ColumnDefinition{Width = new GridLength(350)},

                                    },
                                    Children=
                                    {
                                        new TextBlock
                                        {
                                            FontSize=20,
                                            VerticalAlignment=VerticalAlignment.Center,
                                            HorizontalAlignment=HorizontalAlignment.Left,
                                            Text=Title,
                                            Foreground=(Brush)(new BrushConverter()).ConvertFrom("#fff"),

                                        }
                                    }
                                }
                            },
                            new Border
                            {
                                BorderThickness=new Thickness(1),
                                BorderBrush=(Brush)(new BrushConverter()).ConvertFrom("#3A7597"),
                                CornerRadius=new CornerRadius(0,0,5,5),
                                Child=
                                stack=new StackPanel
                                {
                                    Margin= new Thickness(10),
                                    Children=
                                    {
                                        new TextBox
                                        {
                                            Padding = new Thickness(10, 20, 10, 20),
                                            Margin=new Thickness(0, 0, 0, 10),
                                            BorderThickness = new Thickness(0),
                                            Background = (Brush)(new BrushConverter()).ConvertFrom("#3B5D71"),
                                            IsReadOnly=true,
                                            Text=TextContent,
                                            FontSize=12,
                                            TextWrapping = TextWrapping.Wrap,
                                            Style = new Style(typeof(TextBox))
                                            {
                                                Setters =
                                                {
                                                    new Setter
                                                    {
                                                        Property = TextBox.TemplateProperty,
                                                        Value = template
                                                    }
                                                },

                                            }
                                        },
                                        (border=new Border
                                        {
                                            CornerRadius = new CornerRadius(5),
                                            Child=
                                            new TextBlock
                                            {
                                                HorizontalAlignment= HorizontalAlignment.Center,
                                                VerticalAlignment= VerticalAlignment.Center,
                                                Foreground=(Brush)(new BrushConverter()).ConvertFrom("#fff"),
                                                Margin=new Thickness(10),
                                                FontSize=13,
                                                Text=EndButton.Text
                                            },
                                            Style=
                                            new Style(typeof(Border))
                                            {
                                                Setters =
                                                {
                                                    new Setter
                                                    {
                                                        Property = Border.BackgroundProperty,
                                                        Value = (Brush)(new BrushConverter()).ConvertFrom("#32749A")
                                                    }
                                                },
                                                Triggers =
                                                {
                                                    new Trigger
                                                    {
                                                        Property=Border.IsMouseOverProperty,
                                                        Value=true,
                                                        Setters=
                                                        {
                                                            new Setter
                                                            {
                                                                Property = Border.BackgroundProperty,
                                                                Value =(Brush)(new BrushConverter()).ConvertFrom("#306A8C")
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                        }),

                                    }

                                }
                            }
                        }
                    },

                }



            };

            if (closePopUps)
            {
                SourceImage close = new SourceImage(@"Images/exit1.ico", @"Images/exit2.ico", ClosePopUpsWarning_Click);
                Image SetImage = new Image
                {
                    Height = 14,
                    Width = 14,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Style = new Style(typeof(Image))
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Image.SourceProperty,
                                Value = new BitmapImage(new Uri(close.FirstSourceImage, UriKind.Absolute))
                            }
                        },
                        Triggers =
                    {
                        new Trigger
                        {
                            Property=Image.IsMouseOverProperty,
                            Value=true,
                            Setters=
                            {
                                new Setter
                                {
                                    Property = Image.SourceProperty,
                                    Value =new BitmapImage(new Uri(close.SecondSourceImage, UriKind.Absolute))
                                }
                            }
                        }
                    }
                    }
                };
                Setgird.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Setgird.Children.Add(SetImage);
                Grid.SetColumn(SetImage, 1);


                SetImage.MouseDown += close.mouseDown;
            }
            if (EndButton.mouseDown != null)
                border.MouseDown += EndButton.mouseDown;
            border.MouseDown += ClosePopUpsWarning_Click;

            return gridPanel;
        }
        static public Grid PopUps_Input(string Title, MyButton EndButton, bool closePopUps = false, MyButton LeftButton = null, MyButton RightButton = null, bool AllBigTextBox = false)
        {

            ControlTemplate template = new ControlTemplate(typeof(TextBox)); ;

            FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));




            FrameworkElementFactory element = new FrameworkElementFactory(typeof(Border));
            element.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            element.SetValue(Border.BorderBrushProperty, (Brush)(new BrushConverter()).ConvertFrom("#3A7597"));
            element.SetValue(Border.CornerRadiusProperty, new CornerRadius(5));
            element.SetValue(Border.PaddingProperty, new Thickness(2));
            gridFactory.AppendChild(element);

            FrameworkElementFactory element1 = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
            element1.SetValue(ScrollViewer.MarginProperty, new Thickness(2));
            gridFactory.AppendChild(element1);
            template.VisualTree = gridFactory;



            StackPanel stackContent = new StackPanel
            {
                Background = Brushes.Transparent,
            };
            ScrollViewer scrollViewer = new ScrollViewer
            {
                Content = stackContent,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            };
            //scrollViewer.SetBinding(ScrollViewer.HeightProperty, MainScroll);
            int MyMaxWidth = 500, MyMinWidth = 200;

            if (AllBigTextBox == true)
            {
                MyMaxWidth = 700;
                MyMinWidth = 200;
            }
            foreach (string textKey in contentPopUps.Keys.ToList())
            {


                stackContent.Children.Add
                (

                    new TextBlock
                    {
                        Text = textKey,
                        FontSize = 13,
                        Margin = new Thickness(0, 0, 0, 3),
                        Foreground = (Brush)(new BrushConverter()).ConvertFrom("#3B5D71"),
                    }
                );
                stackContent.Children.Add
                (
                    contentPopUps[textKey].textBox = new TextBox
                    {
                        Margin = new Thickness(0, 0, 0, 5),
                        Padding = new Thickness(0, 5, 5, 5),
                        BorderThickness = new Thickness(0),

                        Background = (Brush)(new BrushConverter()).ConvertFrom("#3B5D71"),
                        MaxLength = contentPopUps[textKey].MaxLength,
                        TextWrapping = TextWrapping.Wrap,

                        Style = new Style(typeof(TextBox))
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = TextBox.TemplateProperty,
                                    Value = template
                                }
                            },

                        }
                    }
                );
            }

            Grid gridContent;
            Border borderEndButton;
            Border borderTitle;
            Border borderContent;
            TextBlock SetBlock;
            Grid Setgird;
            Grid gridPanel = new Grid
            {

                Children =
                {

                    new Grid
                    {
                        Background = (Brush)(new BrushConverter()).ConvertFrom("#F5F5F5"),
                        Opacity = 0.5
                    },
                    new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        MaxWidth=MyMaxWidth,
                        MinWidth=MyMinWidth,
                        Margin = new Thickness(20),
                        Background= (Brush)(new BrushConverter()).ConvertFrom("#fff"),
                        RowDefinitions=
                        {
                            new RowDefinition{ Height = new GridLength(1, GridUnitType.Auto) },
                            new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) }

                        },
                        Children=
                        {
                            (borderTitle=new Border
                            {
                                Background=(Brush)(new BrushConverter()).ConvertFrom("#3A7597"),
                                CornerRadius=new CornerRadius(5,5,0,0),
                                Child=
                                Setgird=new Grid
                                {
                                    Margin=new Thickness(10),
                                    ColumnDefinitions=
                                    {
                                        new ColumnDefinition{Width = new GridLength(350)},

                                    },
                                    Children=
                                    {
                                        (SetBlock =new TextBlock
                                        {
                                            FontSize=20,
                                            VerticalAlignment=VerticalAlignment.Center,
                                            HorizontalAlignment=HorizontalAlignment.Left,
                                            Text=Title,
                                            Foreground=(Brush)(new BrushConverter()).ConvertFrom("#fff"),

                                        })
                                    }
                                }
                            }),
                            (borderContent=new Border
                            {
                                BorderThickness=new Thickness(1),
                                BorderBrush=(Brush)(new BrushConverter()).ConvertFrom("#3A7597"),
                                CornerRadius=new CornerRadius(0,0,5,5),
                                Child=
                                gridContent=new Grid
                                {
                                    RowDefinitions=
                                    {
                                        new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) },
                                        new RowDefinition{ Height = new GridLength(1, GridUnitType.Auto) }


                                    },
                                    Margin= new Thickness(10),
                                    Children=
                                    {
                                        scrollViewer,
                                        (borderEndButton=new Border
                                        {
                                            Margin = new Thickness(0, 5, 0, 10),
                                            CornerRadius = new CornerRadius(5),
                                            Child=
                                            new TextBlock
                                            {
                                                HorizontalAlignment= HorizontalAlignment.Center,
                                                VerticalAlignment= VerticalAlignment.Center,
                                                Foreground=(Brush)(new BrushConverter()).ConvertFrom("#fff"),
                                                Margin=new Thickness(10),
                                                FontSize=13,
                                                Text=EndButton.Text
                                            },
                                            Style=
                                            new Style(typeof(Border))
                                            {
                                                Setters =
                                                {
                                                    new Setter
                                                    {
                                                        Property = Border.BackgroundProperty,
                                                        Value = (Brush)(new BrushConverter()).ConvertFrom("#32749A")
                                                    }
                                                },
                                                Triggers =
                                                {
                                                    new Trigger
                                                    {
                                                        Property=Border.IsMouseOverProperty,
                                                        Value=true,
                                                        Setters=
                                                        {
                                                            new Setter
                                                            {
                                                                Property = Border.BackgroundProperty,
                                                                Value =(Brush)(new BrushConverter()).ConvertFrom("#306A8C")
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                        }),

                                    }

                                }
                            })
                        }
                    },

                }



            };
            Grid.SetColumn(SetBlock, 0);

            Grid.SetRow(borderEndButton, 1);

            Grid.SetRow(borderContent, 1);

            borderEndButton.MouseDown += EndButton.mouseDown;


            if (closePopUps==true)
            {
                SourceImage close = new SourceImage(@"Images/exit1.ico", @"Images/exit2.ico", ClosePopUps_Click);
                Image SetImage = new Image
                {
                    Height = 14,
                    Width = 14,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Style = new Style(typeof(Image))
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Image.SourceProperty,
                                Value = new BitmapImage(new Uri(close.FirstSourceImage, UriKind.Absolute))
                            }
                        },
                        Triggers =
                    {
                        new Trigger
                        {
                            Property=Image.IsMouseOverProperty,
                            Value=true,
                            Setters=
                            {
                                new Setter
                                {
                                    Property = Image.SourceProperty,
                                    Value =new BitmapImage(new Uri(close.SecondSourceImage, UriKind.Absolute))
                                }
                            }
                        }
                    }
                    }
                };
                Setgird.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                Setgird.Children.Add(SetImage);
                Grid.SetColumn(SetImage, 1);
                SetImage.MouseDown += close.mouseDown;
            }





            Grid gridButton = new Grid();
            Style styleTextBlock = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter
                    {
                        Property = TextBlock.ForegroundProperty,
                        Value = (Brush)(new BrushConverter()).ConvertFrom("#2A5885")
                    }
                },
                Triggers =
                {
                    new Trigger
                    {
                        Property=TextBlock.IsMouseOverProperty,
                        Value=true,
                        Setters=
                        {
                            new Setter
                            {
                                Property = TextBlock.ForegroundProperty,
                                Value =(Brush)(new BrushConverter()).ConvertFrom("#3173B4")
                            }
                        }
                    }
                }
            };
            if (LeftButton != null)
            {
                gridButton.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 13,
                    Style = styleTextBlock,
                    Text = LeftButton.Text
                };
                gridButton.Children.Add(textBlock);
                Grid.SetColumn(textBlock, 0);
                if (LeftButton.mouseDown != null)
                {
                    textBlock.MouseDown += LeftButton.mouseDown;
                }
            }
            if (RightButton != null)
            {
                gridButton.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 13,
                    Style = styleTextBlock,
                    Text = RightButton.Text
                };
                gridButton.Children.Add(textBlock);
                Grid.SetColumn(textBlock, 1);
                if (RightButton.mouseDown != null)
                {
                    textBlock.MouseDown += RightButton.mouseDown;
                }

            }
            if (LeftButton != null || RightButton != null)
            {
                gridContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                gridContent.Children.Add(gridButton);
                Grid.SetRow(gridButton, 2);
            }
            return gridPanel;
        }



        static TextBlock Last;
        static void ChangeTab(TextBlock textBlock)
        {
            if (textBlock != Last)
            {
                textBlock.FontWeight = FontWeights.Bold;

                if (Last != null)
                    Last.FontWeight = FontWeights.Normal;

                Last = textBlock;
            }
        }
        static public void Tabs_Click(object sender, MouseButtonEventArgs e)
        {
            ChangeTab((TextBlock)sender);
        }
        static public StackPanel TabsHeadCreate(List<MyTextBlock> Tabs)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center

            };

            for (int i = 0; i < Tabs.Count; i++)
            {

                TextBlock textBlock = new TextBlock
                {
                    Margin = new Thickness(0, 0, 10, 0),
                    FontSize = 15,
                    Text = Tabs[i].Text,
                    Style = new Style(typeof(TextBlock))
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = TextBlock.ForegroundProperty,
                                Value = (Brush)(new BrushConverter()).ConvertFrom("#FFF")
                            }
                        },
                        Triggers =
                        {
                            new Trigger
                            {
                                Property=TextBlock.IsMouseOverProperty,
                                Value=true,
                                Setters=
                                {
                                    new Setter
                                    {
                                        Property = TextBlock.ForegroundProperty,
                                        Value =(Brush)(new BrushConverter()).ConvertFrom("#F5F5F5")
                                    }
                                }
                            }
                        }
                    }
                };
                if (Tabs[i].Select == true)
                {
                    ChangeTab(textBlock);
                }
                textBlock.MouseDown += Tabs_Click;
                textBlock.MouseDown += Tabs[i].mouseDown;

                stackPanel.Children.Add(textBlock);
            }
            return stackPanel;
        }


        static public Border CreateMainTitle(string Title, SourceImage sourceImage = null)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 10, 10, 20),
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock()
                    {
                        Text = Title,
                        Foreground = (Brush)(new BrushConverter()).ConvertFrom("#2C5C78"),
                        FontSize = 25,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 10, 0),
                    }
                }
            };
            if (sourceImage != null)
            {
                stackPanel.Children.Add(new Image()
                {
                    Margin = new Thickness(0, 7, 10, 0),
                    Height = 20,
                    Width = 20,
                    Style = new Style(typeof(Image))
                    {
                        Setters =
                        {
                            new Setter
                            {
                                Property = Image.SourceProperty,
                                Value = new BitmapImage(new Uri(sourceImage.FirstSourceImage, UriKind.Absolute))
                            }
                        },
                        Triggers =
                        {
                            new Trigger
                            {
                                Property=Image.IsMouseOverProperty,
                                Value=true,
                               Setters=
                               {
                                   new Setter
                                   {
                                        Property = Image.SourceProperty,
                                        Value =new BitmapImage(new Uri(sourceImage.SecondSourceImage, UriKind.Absolute))
                                   }
                               }
                            }
                        }
                    }
                });
            }
            return new Border
            {
                BorderBrush = (Brush)(new BrushConverter()).ConvertFrom("#D1D1D1"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10, 10, 10, 0),
                Child = stackPanel,
            };

        }

        static public StackPanel CreateContent(List<MyButton> Buttons)
        {
            StackPanel MainStack = new StackPanel()
            {
                Margin = new Thickness(10, 0, 10, 10),
            };
            for (int i = 0; i < Buttons.Count; i++)
            {
                Border border;
                MainStack.Children.Add
                (
                    border = new Border
                    {
                        BorderBrush = (Brush)(new BrushConverter()).ConvertFrom("#D1D1D1"),
                        BorderThickness = new Thickness(1),

                        Child = new TextBlock
                        {
                            Text = Buttons[i].Text,
                            FontSize = 15,
                            Margin = new Thickness(10)
                        },
                        Style = new Style(typeof(Border))
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = Border.BackgroundProperty,
                                    Value = (Brush)(new BrushConverter()).ConvertFrom("#fff")
                                }
                            },
                            Triggers =
                            {
                                new Trigger
                                {
                                   Property=Border.IsMouseOverProperty,
                                   Value=true,
                                   Setters=
                                   {
                                       new Setter
                                       {
                                            Property = Border.BackgroundProperty,
                                            Value =(Brush)(new BrushConverter()).ConvertFrom("#F2F2F2")
                                       }
                                   }
                                },

                            }
                        },


                    }
                );
                border.MouseDown += Buttons[i].mouseDown;
            }
            return MainStack;
        }


        static public object MyTag { get; set; }
        static public void myButtonBorder_Click(object sender, MouseButtonEventArgs e)
        {
            MyTag = ((Border)sender).Tag;
        }
        static public void myButtonStackPanel_Click(object sender, MouseButtonEventArgs e)
        {
            MyTag = ((StackPanel)sender).Tag;
        }
        static public StackPanel CreateContent(List<string> Titles, NpgsqlDataReader rdr, List<MyButton> myButtons = null, MouseButtonEventHandler mouseDown = null, bool MainTitle = false)
        {
            StackPanel MainStack = new StackPanel
            {
                Margin = new Thickness(10, 0, 10, 10),


            };
            int j = 0;
            if (myButtons != null || mouseDown != null)
            {
                contentMain = new Dictionary<object, List<object>>();
            }
            while (rdr.Read())
            {
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(10)
                };
                if (mouseDown != null)
                {
                    stackPanel.MouseDown += myButtonStackPanel_Click;
                    stackPanel.MouseDown += mouseDown;
                    stackPanel.Tag = j;
                }
                int i = 0;
                if (myButtons != null || mouseDown != null)
                {
                    contentMain.Add(j, new List<object>());
                }
                if (MainTitle == true)
                {
                    TextBlock textBlock;
                    stackPanel.Children.Add
                    (
                        textBlock = new TextBlock
                        {
                            Text = $"{Titles[0]} №{rdr.GetValue(i)}",
                            Foreground = (Brush)(new BrushConverter()).ConvertFrom("#2C5C78"),
                            FontWeight = FontWeights.Bold,
                            FontSize = 25,
                            Margin = new Thickness(0, 0, 0, 5)
                        }
                    );
                    if (myButtons != null || mouseDown != null)
                    {
                        contentMain[j].Add(rdr.GetValue(i));

                    }
                    i++;
                }

                for (; i < Titles.Count; i++)
                {
                    TextBlock textBlock;
                    stackPanel.Children.Add
                    (
                        new TextBlock
                        {
                            Text = Titles[i] + ":",
                            Foreground = (Brush)(new BrushConverter()).ConvertFrom("#3A7597"),
                            FontWeight = FontWeights.Bold,
                            FontSize = 20
                        }
                    );
                    stackPanel.Children.Add
                    (
                        textBlock = new TextBlock
                        {
                            Text = rdr.GetValue(i).ToString(),
                            FontSize = 15
                        }
                    );
                    if (myButtons != null || mouseDown != null)
                    {
                        contentMain[j].Add(textBlock);

                    }
                }
                if (myButtons != null)
                {
                    StackPanel stackButton = new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Orientation = Orientation.Horizontal
                    };
                    for (i = 0; i < myButtons.Count; i++)
                    {
                        Border BorderButton;
                        stackButton.Children.Add
                        (
                            BorderButton = new Border
                            {
                                Tag = j,
                                CornerRadius = new CornerRadius(3),
                                Padding = new Thickness(15, 5, 15, 5),
                                Margin = new Thickness(5, 0, 0, 0),
                                Child =
                                new TextBlock
                                {
                                    Text = myButtons[i].Text,
                                    FontSize = 13,
                                    Foreground = (Brush)(new BrushConverter()).ConvertFrom("#fff"),
                                },
                                Style = new Style(typeof(Border))
                                {
                                    Setters =
                                    {
                                    new Setter
                                    {
                                        Property = Border.BackgroundProperty,
                                        Value = (Brush)(new BrushConverter()).ConvertFrom("#3A7597")
                                    }
                                    },
                                    Triggers =
                                    {
                                    new Trigger
                                    {
                                       Property=Border.IsMouseOverProperty,
                                       Value=true,
                                       Setters=
                                       {
                                           new Setter
                                           {
                                                Property = Border.BackgroundProperty,
                                                Value =(Brush)(new BrushConverter()).ConvertFrom("#306A8C")
                                           }
                                       }
                                    },

                                    }
                                }
                            }

                        );
                        BorderButton.MouseDown += myButtonBorder_Click;
                        BorderButton.MouseDown += myButtons[i].mouseDown;
                    }

                    stackPanel.Children.Add(stackButton);

                }

                MainStack.Children.Add
                (
                    new Border
                    {
                        BorderBrush = (Brush)(new BrushConverter()).ConvertFrom("#D1D1D1"),
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(0, 0, 0, 10),
                        Child = stackPanel,
                        Style = mouseDown == null ? null : new Style(typeof(Border))
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = Border.BackgroundProperty,
                                    Value = (Brush)(new BrushConverter()).ConvertFrom("#fff")
                                }
                            },
                            Triggers =
                            {
                                new Trigger
                                {
                                   Property=Border.IsMouseOverProperty,
                                   Value=true,
                                   Setters=
                                   {
                                       new Setter
                                       {
                                            Property = Border.BackgroundProperty,
                                            Value =(Brush)(new BrushConverter()).ConvertFrom("#F2F2F2")
                                       }
                                   }
                                },

                            }
                        },


                    }
                );
                j++;
            }
            rdr.Close();


            return MainStack;
        }
    }

}
