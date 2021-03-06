﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Configuration.ConfigurationPublic
File: Extensions.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Windows;

	using Ecng.Collections;
	using Ecng.Common;
	using Ecng.Configuration;
	using Ecng.Reflection;
	using Ecng.Serialization;
	using Ecng.Xaml;

	using StockSharp.Algo;
	using StockSharp.Algo.Candles;
	using StockSharp.Algo.Indicators;
	using StockSharp.Xaml;
	using StockSharp.Xaml.Charting;
	using StockSharp.Xaml.Charting.IndicatorPainters;
	using StockSharp.Logging;
	using StockSharp.Messages;
	using StockSharp.AlfaDirect;
	using StockSharp.AlorHistory;
	using StockSharp.AlphaVantage;
	using StockSharp.BarChart;
	using StockSharp.Bibox;
	using StockSharp.Binance;
	using StockSharp.Bitbank;
	using StockSharp.Bitexbook;
	using StockSharp.Bitfinex;
	using StockSharp.Bithumb;
	using StockSharp.Bitmex;
	using StockSharp.BitMax;
	using StockSharp.BitStamp;
	using StockSharp.Bittrex;
	using StockSharp.BitZ;
	using StockSharp.Blackwood;
	using StockSharp.Btce;
	using StockSharp.BW;
	using StockSharp.Cex;
	using StockSharp.Coinbase;
	using StockSharp.CoinBene;
	using StockSharp.Coincheck;
	using StockSharp.CoinExchange;
	using StockSharp.Cqg.Continuum;
	using StockSharp.Cqg.Com;
	using StockSharp.Cryptopia;
	using StockSharp.Deribit;
	using StockSharp.Digifinex;
	using StockSharp.ETrade;
	using StockSharp.Exmo;
	using StockSharp.Fix;
	using StockSharp.Fxcm;
	using StockSharp.Gdax;
	using StockSharp.HitBtc;
	using StockSharp.Huobi;
	using StockSharp.Idax;
	using StockSharp.IEX;
	using StockSharp.InteractiveBrokers;
	using StockSharp.IQFeed;
	using StockSharp.ITCH;
	using StockSharp.Kraken;
	using StockSharp.Kucoin;
	using StockSharp.Liqui;
	using StockSharp.LiveCoin;
	using StockSharp.LMAX;
	using StockSharp.Micex;
	using StockSharp.Oanda;
	using StockSharp.Okcoin;
	using StockSharp.Okex;
	using StockSharp.OpenECry;
	using StockSharp.Plaza;
	using StockSharp.Poloniex;
	using StockSharp.QuantHouse;
	using StockSharp.Quik;
	using StockSharp.Quik.Lua;
	using StockSharp.Quoinex;
	using StockSharp.Rithmic;
	using StockSharp.Rss;
	using StockSharp.SmartCom;
	using StockSharp.SpbEx;
	using StockSharp.Sterling;
	using StockSharp.TradeOgre;
	using StockSharp.Transaq;
	using StockSharp.Twime;
	using StockSharp.Yobit;
	using StockSharp.Zaif;
	using StockSharp.CoinCap;
	using StockSharp.Coinigy;
	using StockSharp.CSV;
	using StockSharp.DukasCopy;
	using StockSharp.Finam;
	using StockSharp.FinViz;
	using StockSharp.Google;
	using StockSharp.LBank;
	using StockSharp.Mfd;
	using StockSharp.Quandl;
	using StockSharp.Tradier;
	using StockSharp.UkrExh;
	using StockSharp.Xignite;
	using StockSharp.Yahoo;
	using StockSharp.ZB;

	/// <summary>
	/// Extension class.
	/// </summary>
	public static class Extensions
	{
		private static readonly IndicatorType[] _customIndicators = ArrayHelper.Empty<IndicatorType>();
		private static readonly Type[] _customCandles = ArrayHelper.Empty<Type>();
		private static readonly Type[] _customDiagramElements = ArrayHelper.Empty<Type>();

		static Extensions()
		{
			var section = RootSection;

			if (section == null)
				return;

			_customIndicators = SafeAdd<IndicatorElement, IndicatorType>(section.CustomIndicators, elem => new IndicatorType(elem.Type.To<Type>(), elem.Painter.To<Type>()));
			_customCandles = SafeAdd<CandleElement, Type>(section.CustomCandles, elem => elem.Type.To<Type>());
			_customDiagramElements = SafeAdd<DiagramElement, Type>(section.CustomDiagramElements, elem => elem.Type.To<Type>());
		}

		/// <summary>
		/// Instance of the root section <see cref="StockSharpSection"/>.
		/// </summary>
		public static StockSharpSection RootSection => ConfigManager.InnerConfig.Sections.OfType<StockSharpSection>().FirstOrDefault();

		private static T2[] SafeAdd<T1, T2>(IEnumerable from, Func<T1, T2> func)
		{
			var list = new List<T2>();

			foreach (T1 item in from)
			{
				try
				{
					list.Add(func(item));
				}
				catch (Exception e)
				{
					e.LogError();
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Configure connection using <see cref="ConnectorWindow"/>.
		/// </summary>
		/// <param name="connector">The connection.</param>
		/// <param name="owner">UI thread owner.</param>
		/// <returns><see langword="true"/> if the specified connection was configured, otherwise, <see langword="false"/>.</returns>
		public static bool Configure(this Connector connector, Window owner)
		{
			if (connector == null)
				throw new ArgumentNullException(nameof(connector));

			return connector.Adapter.Configure(owner);
		}

		/// <summary>
		/// Configure connection using <see cref="ConnectorWindow"/>.
		/// </summary>
		/// <param name="adapter">The connection.</param>
		/// <param name="owner">UI thread owner.</param>
		/// <returns><see langword="true"/> if the specified connection was configured, otherwise, <see langword="false"/>.</returns>
		public static bool Configure(this BasketMessageAdapter adapter, Window owner)
		{
			var autoConnect = false;
			SettingsStorage settings = null;
			return adapter.Configure(owner, ref autoConnect, ref settings);
		}

		private static readonly Lazy<Func<Type>[]> _standardAdapters = new Lazy<Func<Type>[]>(() => new[]
		{
			(Func<Type>)(() => typeof(AlfaDirectMessageAdapter)),
			() => typeof(BarChartMessageAdapter),
			() => typeof(BitStampMessageAdapter),
			() => typeof(BlackwoodMessageAdapter),
			() => typeof(BtceMessageAdapter),
			() => typeof(CqgComMessageAdapter),
			() => typeof(CqgContinuumMessageAdapter),
			() => typeof(ETradeMessageAdapter),
			() => typeof(FixMessageAdapter),
			() => typeof(FastMessageAdapter),
			() => typeof(InteractiveBrokersMessageAdapter),
			() => typeof(IQFeedMessageAdapter),
			() => typeof(ItchMessageAdapter),
			() => typeof(LmaxMessageAdapter),
			() => typeof(MicexMessageAdapter),
			() => typeof(OandaMessageAdapter),
			() => typeof(OpenECryMessageAdapter),
			() => typeof(PlazaMessageAdapter),
			() => typeof(LuaFixTransactionMessageAdapter),
			() => typeof(LuaFixMarketDataMessageAdapter),
			() => typeof(QuikTrans2QuikAdapter),
			() => typeof(QuikDdeAdapter),
			() => typeof(RithmicMessageAdapter),
			() => typeof(RssMessageAdapter),
			() => typeof(SmartComMessageAdapter),
			() => typeof(SterlingMessageAdapter),
			() => typeof(TransaqMessageAdapter),
			() => typeof(TwimeMessageAdapter),
			() => typeof(SpbExMessageAdapter),
			() => typeof(FxcmMessageAdapter),
			() => typeof(QuantFeedMessageAdapter),
			() => typeof(BitfinexMessageAdapter),
			() => typeof(BithumbMessageAdapter),
			() => typeof(BittrexMessageAdapter),
			() => typeof(CoinbaseMessageAdapter),
			() => typeof(CoincheckMessageAdapter),
			() => typeof(GdaxMessageAdapter),
			() => typeof(HitBtcMessageAdapter),
			() => typeof(KrakenMessageAdapter),
			() => typeof(OkcoinMessageAdapter),
			() => typeof(PoloniexMessageAdapter),
			() => typeof(BinanceMessageAdapter),
			() => typeof(BitexbookMessageAdapter),
			() => typeof(BitmexMessageAdapter),
			() => typeof(CexMessageAdapter),
			() => typeof(CoinExchangeMessageAdapter),
			() => typeof(CryptopiaMessageAdapter),
			() => typeof(DeribitMessageAdapter),
			() => typeof(ExmoMessageAdapter),
			() => typeof(HuobiMessageAdapter),
			() => typeof(KucoinMessageAdapter),
			() => typeof(LiquiMessageAdapter),
			() => typeof(LiveCoinMessageAdapter),
			() => typeof(OkexMessageAdapter),
			() => typeof(YobitMessageAdapter),
			() => typeof(AlphaVantageMessageAdapter),
			() => typeof(IEXMessageAdapter),
			() => typeof(QuoinexMessageAdapter),
			() => typeof(BitbankMessageAdapter),
			() => typeof(ZaifMessageAdapter),
			() => typeof(DigifinexMessageAdapter),
			() => typeof(IdaxMessageAdapter),
			() => typeof(TradeOgreMessageAdapter),
			() => typeof(CoinCapMessageAdapter),
			() => typeof(CoinigyMessageAdapter),
			() => typeof(LBankMessageAdapter),
			() => typeof(BitMaxMessageAdapter),
			() => typeof(BWMessageAdapter),
			() => typeof(BiboxMessageAdapter),
			() => typeof(CoinBeneMessageAdapter),
			() => typeof(BitZMessageAdapter),
			() => typeof(ZBMessageAdapter),
			() => typeof(TradierMessageAdapter),
			() => typeof(DukasCopyMessageAdapter),
			() => typeof(FinamMessageAdapter),
			() => typeof(AlorHistoryMessageAdapter),
			() => typeof(MfdMessageAdapter),
			() => typeof(QuandlMessageAdapter),
			() => typeof(XigniteMessageAdapter),
			() => typeof(YahooMessageAdapter),
			() => typeof(GoogleMessageAdapter),
			() => typeof(FinVizMessageAdapter),
			() => typeof(UkrExhMessageAdapter),
			() => typeof(CSVMessageAdapter),
		});
		
		private static readonly SyncObject _adaptersLock = new SyncObject();
		private static Type[] _adapters;

		/// <summary>
		/// All available adapters.
		/// </summary>
		public static IEnumerable<Type> Adapters
		{
			get
			{
				lock (_adaptersLock)
				{
					if (_adapters == null)
					{
						var exceptions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
						{
							"StockSharp.Alerts",
							"StockSharp.Algo",
							"StockSharp.Algo.History",
							"StockSharp.Algo.Strategies",
							"StockSharp.BusinessEntities",
							"StockSharp.Community",
							"StockSharp.Configuration",
							"StockSharp.Licensing",
							"StockSharp.Localization",
							"StockSharp.Logging",
							"StockSharp.Messages",
							"StockSharp.Xaml",
							"StockSharp.Xaml.Actipro",
							"StockSharp.Xaml.Charting",
							"StockSharp.Xaml.Diagram",
							"StockSharp.Studio.Core",
							"StockSharp.Studio.Controls"
						};

						var adapters = new List<Type>();

						foreach (var func in _standardAdapters.Value)
						{
							try
							{
								var type = func();

								exceptions.Add(type.Assembly.GetName().Name);

								if (type == typeof(QuikDdeAdapter) || type == typeof(QuikTrans2QuikAdapter))
									continue;

								adapters.Add(type);
							}
							catch (Exception e)
							{
								e.LogError();
							}
						}

						var assemblies = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll").Where(p =>
						{
							var name = Path.GetFileNameWithoutExtension(p);
							return !exceptions.Contains(name) && name.StartsWithIgnoreCase("StockSharp.");
						});

						foreach (var assembly in assemblies)
						{
							if (!assembly.IsAssembly())
								continue;

							try
							{
								var asm = Assembly.Load(AssemblyName.GetAssemblyName(assembly));

								adapters.AddRange(asm
									.GetTypes()
									.Where(t => typeof(IMessageAdapter).IsAssignableFrom(t) && !t.IsAbstract)
									.ToArray());
							}
							catch (Exception e)
							{
								e.LogError();
							}
						}

						_adapters = adapters.ToArray();
					}
				}

				return _adapters;
			}
		}

		/// <summary>
		/// Configure connection using <see cref="ConnectorWindow"/>.
		/// </summary>
		/// <param name="adapter">The connection.</param>
		/// <param name="owner">UI thread owner.</param>
		/// <param name="autoConnect">Auto connect.</param>
		/// <param name="windowSettings"><see cref="ConnectorWindow"/> settings.</param>
		/// <returns><see langword="true"/> if the specified connection was configured, otherwise, <see langword="false"/>.</returns>
		public static bool Configure(this BasketMessageAdapter adapter, Window owner, ref bool autoConnect, ref SettingsStorage windowSettings)
		{
			if (adapter == null)
				throw new ArgumentNullException(nameof(adapter));

			if (owner == null)
				throw new ArgumentNullException(nameof(owner));

			var wnd = new ConnectorWindow();

			if (windowSettings != null)
				wnd.Load(windowSettings);

			foreach (var a in Adapters)
			{
				AddConnectorInfo(wnd, a);
			}

			wnd.Adapter = (BasketMessageAdapter)adapter.Clone();
			wnd.AutoConnect = autoConnect;

			if (!wnd.ShowModal(owner))
			{
				windowSettings = wnd.Save();
				return false;
			}

			adapter.Load(wnd.Adapter.Save());
			autoConnect = wnd.AutoConnect;
			windowSettings = wnd.Save();

			return true;
		}

		private static void AddConnectorInfo(ConnectorWindow wnd, Type adapterType)
		{
			if (wnd == null)
				throw new ArgumentNullException(nameof(wnd));

			wnd.ConnectorsInfo.Add(new ConnectorInfo(adapterType));
		}

		private static IndicatorType[] _indicatorTypes;

		/// <summary>
		/// Get all indicator types.
		/// </summary>
		/// <returns>All indicator types.</returns>
		public static IEnumerable<IndicatorType> GetIndicatorTypes()
		{
			if (_indicatorTypes == null)
			{
				var ns = typeof(IIndicator).Namespace;

				var rendererTypes = typeof(Chart).Assembly
					.GetTypes()
					.Where(t => !t.IsAbstract && typeof(BaseChartIndicatorPainter).IsAssignableFrom(t) && t.GetAttribute<IndicatorAttribute>() != null)
					.ToDictionary(t => t.GetAttribute<IndicatorAttribute>().Type);

				_indicatorTypes = typeof(IIndicator).Assembly
					.GetTypes()
					.Where(t => t.Namespace == ns && !t.IsAbstract && typeof(IIndicator).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null && t.GetAttribute<BrowsableAttribute>()?.Browsable != false)
					.Select(t => new IndicatorType(t, rendererTypes.TryGetValue(t)))
					.Concat(_customIndicators)
					.OrderBy(t => t.Name)
					.ToArray();
			}

			return _indicatorTypes;
		}

		/// <summary>
		/// Fill <see cref="IChart.IndicatorTypes"/> using <see cref="GetIndicatorTypes"/>.
		/// </summary>
		/// <param name="chart">Chart.</param>
		public static void FillIndicators(this IChart chart)
		{
			if (chart == null)
				throw new ArgumentNullException(nameof(chart));

			chart.IndicatorTypes.Clear();
			chart.IndicatorTypes.AddRange(GetIndicatorTypes());
		}

		private static Type[] _diagramElements;

		/// <summary>
		/// Get all diagram elements.
		/// </summary>
		/// <returns>All diagram elements.</returns>
		public static IEnumerable<Xaml.Diagram.DiagramElement> GetDiagramElements()
		{
			if (_diagramElements == null)
			{
				_diagramElements = typeof(Xaml.Diagram.DiagramElement).Assembly
					.GetTypes()
					.Where(t => !t.IsAbstract && 
						t.IsSubclassOf(typeof(Xaml.Diagram.DiagramElement)) && 
						t != typeof(Xaml.Diagram.CompositionDiagramElement))
					.Concat(_customDiagramElements)
					.OrderBy(t => t.Name)
					.ToArray();
			}

			return _diagramElements
				.Select(t => t.CreateInstance<Xaml.Diagram.DiagramElement>())
				.ToArray();
		}

		private static Type[] _candles;

		/// <summary>
		/// Get all candles.
		/// </summary>
		/// <returns>All candles.</returns>
		public static IEnumerable<Type> GetCandles()
		{
			return _candles ?? (_candles = typeof(Candle).Assembly
				.GetTypes()
				.Where(t => !t.IsAbstract && t.IsCandle())
				.Concat(_customCandles)
				.ToArray());
		}
	}
}