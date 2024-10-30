using GameName.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StockSlotView : MonoBehaviour, IUpdatable
{
	[Space]
	[SerializeField]
	protected float m_padding;
	[SerializeField]
	protected float m_spacing;

	protected StockSlotData _data;

	protected Cell[] _cells;

	protected List<FallHandle> _fallHandles;

	protected SharedViewData _viewData;

	private void Awake()
	{
		_fallHandles = new List<FallHandle>();
	}

	public void Init(StockSlotData data, SharedViewData viewData)
	{
		_data = data;
		_viewData = viewData;

		GenerateCells();
	}

	public void OnUpdate(float deltaTime)
	{
		OnUpdateTransforms(deltaTime);
	}

	protected virtual void GenerateCells()
	{
		_cells = new Cell[_data.capacity];

		for (int i = 0; i < _data.capacity; i++)
		{
			GameObject go = new GameObject($"cell_{i:D2}");

			go.transform.SetParent(transform, false);
			go.transform.localPosition = (m_padding * Vector3.up) + ((i * m_spacing) * Vector3.up);
			
			_cells[i] = new Cell
			{
				target = go.transform,
				isOccupied = false,
				isGhost = false,
				gameObject = null
			};
		}
	}

	public class Cell
	{
		public Transform target;
		public bool isOccupied;
		public bool isGhost;
		public GameObject gameObject;
	}

	public Cell GetCell(int index) => _cells[index];

	public virtual void FillCell(int index, GameObject gameObject, bool isGhost)
	{
		FillCell(_cells[index], gameObject, isGhost);
	}
	public virtual void FillCell(Cell cell, GameObject gameObject, bool isGhost)
	{
		cell.gameObject = gameObject;
		cell.isOccupied = true;
		cell.isGhost = isGhost;
	}
	public virtual void ClearCell(int index)
	{
		ClearCell(_cells[index]);
	}
	public virtual void ClearCell(Cell cell)
	{
		cell.isOccupied = false;
		cell.isGhost = false;
		cell.gameObject = null;
	}

	public virtual void UpdateOrder(int index)
	{
		for (int i = index; i < _cells.Length - 1; i++)
		{
			_cells[i].isOccupied = _cells[i + 1].isOccupied;
			_cells[i].isGhost = _cells[i + 1].isGhost;
			_cells[i].gameObject = _cells[i + 1].gameObject;

			ClearCell(_cells[i + 1]);

			if (_cells[i].isOccupied)
			{
				UpdateTransform(_cells[i].gameObject.transform, _cells[i].target);
			}
		}
	}

	protected virtual void UpdateTransform(Transform objectTransform, Transform targetTransform)
	{
		FallHandle fallHandle = new FallHandle
		{
			data = _data.fallData,
			objectTransform = objectTransform,
			targetTransform = targetTransform
		};

		int index = _fallHandles.IndexOf(fallHandle);
		if (index != -1)
		{
			_fallHandles[index] = fallHandle;
		}
		else
		{
			_fallHandles.Add(fallHandle);
		}
	}

	protected virtual void OnUpdateTransforms(float deltaTime)
	{
		for (int i = _fallHandles.Count - 1; i >= 0; i--)
		{
			if (_fallHandles[i].Process(deltaTime))
			{
				_fallHandles.RemoveAt(i);
			}
		}
	}

	protected class FallHandle : IEquatable<FallHandle>
	{
		public StockSlotFallData data;

		public Transform objectTransform;
		public Transform targetTransform;

		private float _time = 0.0f;
		
		public bool Process(float deltaTime)
		{
			_time += deltaTime;

			float t = Mathf.Clamp01(data.positionCurve.Evaluate(_time / data.duration));

			objectTransform.position = Vector3.Lerp(objectTransform.position, targetTransform.position, t);

			return _time >= data.duration;
		}

		public bool Equals(FallHandle other)
		{
			return objectTransform.Equals(other.objectTransform);
		}
	}
}