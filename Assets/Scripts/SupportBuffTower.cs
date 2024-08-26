using System.Collections.Generic;
using UnityEngine;

public class SupportBuffTower : MonoBehaviour
{
	public List<SupportBuff> supportBuffs = new List<SupportBuff>();
	[SerializeField] private float buffCheckInterval = 1.0f; // ���� üũ ����
	[SerializeField] private float buffLifeTime; // ���� ���� �ð�

	private void Start()
	{
		InvokeRepeating("CheckSupportBuffs", 0f, buffCheckInterval);
	}

	private void CheckSupportBuffs()
	{
		float currentTime = Time.time;
		for (int i = supportBuffs.Count - 1; i >= 0; i--)
		{
			SupportBuff _Buff = supportBuffs[i];
			if (currentTime - _Buff.lastAppliedTime > buffLifeTime)
			{
				Debug.Log($"���� �ð� {currentTime} / ������ ���� {_Buff.lastAppliedTime} : {currentTime - _Buff.lastAppliedTime > buffLifeTime}");
				RemoveSupportBuff(_Buff);
			}
		}
	}

	public void RemoveSupportBuff(SupportBuff _Buff)
	{
		Debug.Log("���� ���� ��...");

		_Buff.tower.Damage -= _Buff.tower.baseDamage * _Buff.buffAmout;
		_Buff.tower.Range -= _Buff.tower.baseRange * _Buff.buffAmout;
		_Buff.tower.Rate += _Buff.tower.baseRate * _Buff.buffAmout; // ���� �ӵ��� ������ �ƴ϶� ���ҿ����Ƿ� ���ݴϴ�.

		supportBuffs.Remove(_Buff);
	}

	public void AddTower(Tower _Tower, float _BuffAmout)
	{
		foreach (SupportBuff _Buff in supportBuffs)
		{
			if (_Buff.tower == _Tower)
			{
				_Buff.buffAmout = _BuffAmout;
				_Buff.lastAppliedTime = Time.time;
				return;
			}
		}

		SupportBuff newTower = new SupportBuff
		{
			tower = _Tower,
			buffAmout = _BuffAmout,
			lastAppliedTime = Time.time
		};

		supportBuffs.Add(newTower);
		ApplyBuff(newTower);
	}

	public void ReapplyAllBuffs()
	{
		foreach (SupportBuff _Buff in supportBuffs)
		{
			ApplyBuff(_Buff);
		}
	}

	private void ApplyBuff(SupportBuff _Buff)
	{
		_Buff.tower.Damage += _Buff.tower.baseDamage * _Buff.buffAmout;
		_Buff.tower.Range += _Buff.tower.baseRange * _Buff.buffAmout;
		_Buff.tower.Rate -= _Buff.tower.baseRate * _Buff.buffAmout; // ���� �ӵ��� ������ �ƴ϶� ���ҿ����Ƿ� ���ݴϴ�.
	}
}

[System.Serializable]
public class SupportBuff
{
	public Tower tower;
	public float buffAmout;
	public float lastAppliedTime;
}
