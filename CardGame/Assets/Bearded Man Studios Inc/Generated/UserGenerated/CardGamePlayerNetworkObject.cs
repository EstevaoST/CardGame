using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class CardGamePlayerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 9;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private bool _Skipping;
		public event FieldEvent<bool> SkippingChanged;
		public Interpolated<bool> SkippingInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool Skipping
		{
			get { return _Skipping; }
			set
			{
				// Don't do anything if the value is the same
				if (_Skipping == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_Skipping = value;
				hasDirtyFields = true;
			}
		}

		public void SetSkippingDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_Skipping(ulong timestep)
		{
			if (SkippingChanged != null) SkippingChanged(_Skipping, timestep);
			if (fieldAltered != null) fieldAltered("Skipping", _Skipping, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			SkippingInterpolation.current = SkippingInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Skipping);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Skipping = UnityObjectMapper.Instance.Map<bool>(payload);
			SkippingInterpolation.current = _Skipping;
			SkippingInterpolation.target = _Skipping;
			RunChange_Skipping(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Skipping);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (SkippingInterpolation.Enabled)
				{
					SkippingInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					SkippingInterpolation.Timestep = timestep;
				}
				else
				{
					_Skipping = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_Skipping(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (SkippingInterpolation.Enabled && !SkippingInterpolation.current.UnityNear(SkippingInterpolation.target, 0.0015f))
			{
				_Skipping = (bool)SkippingInterpolation.Interpolate();
				//RunChange_Skipping(SkippingInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public CardGamePlayerNetworkObject() : base() { Initialize(); }
		public CardGamePlayerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public CardGamePlayerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
