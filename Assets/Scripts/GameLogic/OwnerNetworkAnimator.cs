using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

/*
 �ִϸ��̼��� ���� ������ �������� ó���ǰ� Ŭ���̾�Ʈ���� ���۵ȴ�.
�׸��� ���� ������ �ʿ��ϴ�. ������ �ִϸ��̼� ���¸� �����ϰ� ������ �ٸ� Ŭ���̾�Ʈ���� �˸��� ���ؼ���.
�̸� ���� ���� �÷��̾�� �ִϸ��̼� ���� ��ȭ�� �ﰢ������ Ȯ���� �� �ְ�, ���� �ǵ���� ���� �� �ִ�.
�̸� ���� ���� ������ ���� ��ũ��Ʈ�� �ϳ� �� ������ �Ѵ�.
 */
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
