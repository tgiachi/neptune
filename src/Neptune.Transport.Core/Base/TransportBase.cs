using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Neptune.Packets.Extensions;
using Neptune.Packets.Messages;
using Neptune.Transport.Core.Events;
using Neptune.Transport.Core.Interfaces;
using Neptune.Transport.Core.Models;

namespace Neptune.Transport.Core.Base;

 /// <summary>
    /// Base implementation for transport providers
    /// </summary>
    public abstract class TransportBase : ITransport
    {
        protected readonly ILogger _logger;
        protected readonly TransportOptions _options;
        protected readonly ConcurrentDictionary<string, PeerInfo> _connectedPeers;
        protected CancellationTokenSource _cts;
        protected bool _isActive;
        protected readonly TransportMetrics _metrics;

        public string TransportId { get; }
        public abstract string TransportType { get; }
        public bool IsActive => _isActive;
        public TransportMetrics Metrics => _metrics;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<PeerDiscoveredEventArgs> PeerDiscovered;
        public event EventHandler<TransportStatusChangedEventArgs> StatusChanged;

        protected TransportBase(ILogger<TransportBase> logger, string transportId, TransportOptions options)
        {
            TransportId = transportId;
            _options = options;
            _logger = logger;
            _connectedPeers = new ConcurrentDictionary<string, PeerInfo>();
            _metrics = new TransportMetrics();
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isActive)
                return;

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await InitializeTransportAsync(_cts.Token);
                _isActive = true;
                _metrics.SetStartTime();
                OnStatusChanged(new TransportStatusChangedEventArgs(this, true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start transport {TransportId}", TransportId);
                throw;
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_isActive)
                return;

            try
            {
                _cts?.Cancel();
                await ShutdownTransportAsync(cancellationToken);
                _isActive = false;
                OnStatusChanged(new TransportStatusChangedEventArgs(this, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping transport {TransportId}", TransportId);
                throw;
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        public abstract Task<bool> SendMessageAsync(NeptuneMessage message, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<PeerInfo>> DiscoverPeersAsync(CancellationToken cancellationToken = default);

        protected abstract Task InitializeTransportAsync(CancellationToken cancellationToken);

        protected abstract Task ShutdownTransportAsync(CancellationToken cancellationToken);

        protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
        {
            _metrics.IncrementMessagesReceived();
            _metrics.AddBytesReceived(args.Message.ToJson().Length);
            MessageReceived?.Invoke(this, args);
        }

        protected virtual void OnPeerDiscovered(PeerDiscoveredEventArgs args)
        {
            PeerDiscovered?.Invoke(this, args);
        }

        protected virtual void OnStatusChanged(TransportStatusChangedEventArgs args)
        {
            StatusChanged?.Invoke(this, args);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_isActive)
                {
                    StopAsync().GetAwaiter().GetResult();
                }

                _cts?.Dispose();
            }
        }
    }
