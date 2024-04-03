// ConsoleApplication1.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include <iostream>

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <conio.h>

#define MAX_PROBABILITY 100.0f            
#define WAIT_TIME 2 

//함수 선언 공간

//강화 변수
int isTry = 0;                          // 강화를 할 것인지 선택
int level = 0;                          // 현재 무기의 레벨 
int totallevel = 0;                     // 지금까지의 무기 최고레벨
float probability = MAX_PROBABILITY;    // 현재 강화 성공확률
time_t retTime = 0;                     // 대기시간 임시저장 변수
int randNum = 0;                        // 랜덤값을 저장할 변수
int support = 0;
bool nobreak = false;

// 5를 누르면 지금까지 나온 무기 중 가장 높은 등급을 알려주도록 만들기


//상점 변수
int money = 500;                          //재화 저장 변수
int weaponvalue = 0;                    //무기 가치 저장 변수


//함수 식별자 선언 
//따로 만든 함수 있으면 여기서 식별자 선언 해주세요

void enforce();
void enforcesupport();
void shop();

int main(void)
{


    srand((int)time(NULL));                 // 랜덤 시드값 설정

    while (1)
    {
        // 화면 정리
        system("@cls||clear");

        // 타이틀 화면 출력
        printf("*= Blacksmith =*\n");
        printf("  -----------\n\n");

        // 현재 상태와 강화 도전 질의 출력
        printf("재화 : %d골드\n\n", money);
        printf("무기레벨 : + %d\n", level);
        printf("성공확률 : %.2f%%\n", (probability + support));
        printf("도전하시겠습니까?\n");
        printf("1.강화  2. 판매  3. 주문서 4. 포기 5. 최고기록\n");
        scanf_s("%d", &isTry);

        switch (isTry)
        {
        case 1:        // 강화에 도전 할 경우
            enforce();
            break;

        case 2:
            shop();
            break;

        case 3:
            enforcesupport();
            break;

        case 4:
            // 포기를 할 경우 프로그램 종료
            printf("게임을 종료합니다\n");
            return -1;

        case 5:
            printf("무기의 최고레벨은 %d 입니다.", totallevel);
            break;
        }

        // 진행상황 확인이 용이 하도록 대기
        printf("\n계속하려면 아무 키나 누르십시오.\n");
        _getch();
    }

    return 0;
}


void enforce()
{
    printf("\n강화중..\n\n");

    // 지정된 시간(초) 만큼 대기
    retTime = time(0) + WAIT_TIME;
    while (time(0) < retTime);

    // 랜덤 값 추출
    randNum = rand() % 100;
    // 추출한 랜덤 값이 성공확률 보다 작으면 성공
    if (randNum < probability + support) {
        // 성공화면 출력
        printf("***** SUCCESS *****\n");
        printf("*                 *\n");
        printf("*   + %d  ->  + %d  *\n", level, level + 1);
        printf("*                 *\n");
        printf("***** SUCCESS *****\n");

        // 강화에 성공 했을 시, 레벨을 하나 증가 시키고, 성공확률을 보정
        // 이 때, 현재 성공확률의 10%에 현재 레벨로 가중치를 추가로 적용
        level++;
        if (totallevel < level)
        {
            totallevel = level;
        }
        support = 0;
        weaponvalue += 500;
        probability -= (probability / 10.0f) * level;
    }
    else
    {
        // 실패화면 출력
        printf("강화 실패..\n");
        if (nobreak == true)
        {
            printf("절대 파괴 안됨의 가호가 치명적인 파괴를 막았습니다.\n");
            printf("가호가 사라집니다.\n");
            nobreak = false;
        }
        else
        {
            printf("+ %d 무기를 잃었습니다.\n", level);

            // 각종 수치 초기화
            level = 0;
            support = 0;
            weaponvalue = 0;
            probability = MAX_PROBABILITY;
        }


    }
}

void enforcesupport()
{
    int supportchoice;
    if (money >= 50) {
        printf("신비의 주문서\n\n");
        printf("1. 무기 파괴 방지서 : %d골드\n", 100 + (level * 100));
        printf("2. 강화 확률 상승서 : %d골드\n", 200 + (level * 200));
        printf("3. 무기 가치 상승서 : %d골드\n", 50 + (level * 50));
        scanf_s("%d", &supportchoice);

        switch (supportchoice)
        {
        case 1: // 무기 파괴 방지서를 선택한 경우
            if (money >= level * 100) {
                if (nobreak == false)
                {
                    printf("무기 파괴 방지서를 구매하였습니다.\n");
                    printf("절대 파괴 안됨의 가호가 깃듭니다.\n");
                    nobreak = true;
                    money -= 100 + (level * 100);
                }
                else {
                    printf("이미 받고 있는 가호입니다.\n");
                }


            }
            else {
                printf("재화가 부족합니다.\n");
            }
            break;
        case 2: // 강화 확률 상승서를 선택한 경우
            if (money >= level * 200) {
                printf("강화 확률 상승서를 구매하였습니다.\n");
                printf("손이 미끄러지는 기운이 멀어집니다.\n");
                printf("성공확률 : %.2f%%\n", (probability + support));
                support += 5;
                money -= 200 + (level * 100);

            }
            else {
                printf("재화가 부족합니다.\n");
            }
            break;
        case 3: // 무기 가치 상승서를 선택한 경우
            if (money >= level * 50) {
                printf("무기 가치 상승서를 구매하였습니다.\n");
                weaponvalue += (50 * level);
                money -= 50 + (level * 100);
                printf("당신에게 재화의 가호가 깃들었습니다.!!\n");
                printf("현재 무기 가치 보정 : %d\n\n", weaponvalue);

            }
            else {
                printf("재화가 부족합니다.\n");
            }
            break;
        default:
            printf("잘못된 선택입니다.\n");
            break;
        }
    }
    else
    {
        printf("재화가 부족합니다.\n");
    }
}

void shop()
{
    if (level > 0) {                     // 레벨이 0보다 크면 무기가 존재함
        int sellvalue = weaponvalue / 2; // 무기의 값을 무기 가치의 절반으로 설정하여 판매

        printf("무기를 판매합니다.\n");
        printf("판매된 무기 레벨: + %d\n", level);
        printf("판매하는 무기의 값: %d골드\n", sellvalue);

        money += sellvalue;            // 플레이어의 재화에 판매된 무기의 값을 추가
        level = 0;                     // 무기 레벨 초기화
        weaponvalue = 0;               // 무기 가치 초기화
        probability = MAX_PROBABILITY; // 강화 성공 확률 초기화
    }
    else {
        printf("판매할 무기가 없습니다.\n");
    }
}

