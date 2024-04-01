// ConsoleApplication1.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include <iostream>

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <conio.h>

#define MAX_PROBABILITY 100.0f            
#define WAIT_TIME 2 

int main(void)
{
    int isTry = 0;                          // 강화를 할 것인지 선택
    int level = 0;                          // 현재 무기의 레벨
    float probability = MAX_PROBABILITY;    // 현재 강화 성공확률
    time_t retTime = 0;                     // 대기시간 임시저장 변수
    int randNum = 0;                        // 랜덤값을 저장할 변수

    srand((int)time(NULL));                 // 랜덤 시드값 설정

    while (1)
    {
        // 화면 정리
        system("@cls||clear");

        // 타이틀 화면 출력
        printf("*= Blacksmith =*\n");
        printf("  -----------\n\n");

        // 현재 상태와 강화 도전 질의 출력
        printf("무기레벨 : + %d\n", level);
        printf("성공확률 : %.2f%%\n", probability);
        printf("도전하시겠습니까?\n");
        printf("1.강화   2.포기\n");
        scanf_s("%d", &isTry);

        switch (isTry)
        {
        case 1:        // 강화에 도전 할 경우
            printf("\n강화중..\n\n");

            // 지정된 시간(초) 만큼 대기
            retTime = time(0) + WAIT_TIME;
            while (time(0) < retTime);

            // 랜덤 값 추출
            randNum = rand() % 100;
            // 추출한 랜덤 값이 성공확률 보다 작으면 성공
            if (randNum < probability) {
                // 성공화면 출력
                printf("***** SUCCESS *****\n");
                printf("*                 *\n");
                printf("*   + %d  ->  + %d  *\n", level, level + 1);
                printf("*                 *\n");
                printf("***** SUCCESS *****\n");

                // 강화에 성공 했을 시, 레벨을 하나 증가 시키고, 성공확률을 보정
                // 이 때, 현재 성공확률의 10%에 현재 레벨로 가중치를 추가로 적용
                level++;
                probability -= (probability / 10.0f) * level;
            }
            else
            {
                // 실패화면 출력
                printf("강화 실패..\n");
                printf("+ %d 무기를 잃었습니다.\n", level);

                // 각종 수치 초기화
                level = 0;
                probability = MAX_PROBABILITY;
            }
            break;

        case 2:
            // 포기를 할 경우 프로그램 종료
            printf("게임을 종료합니다\n");
            return -1;
        }

        // 진행상황 확인이 용이 하도록 대기
        printf("\n계속하려면 아무 키나 누르십시오.\n");
        _getch();
    }

    return 0;
}



// 프로그램 실행: <Ctrl+F5> 또는 [디버그] > [디버깅하지 않고 시작] 메뉴
// 프로그램 디버그: <F5> 키 또는 [디버그] > [디버깅 시작] 메뉴

// 시작을 위한 팁: 
//   1. [솔루션 탐색기] 창을 사용하여 파일을 추가/관리합니다.
//   2. [팀 탐색기] 창을 사용하여 소스 제어에 연결합니다.
//   3. [출력] 창을 사용하여 빌드 출력 및 기타 메시지를 확인합니다.
//   4. [오류 목록] 창을 사용하여 오류를 봅니다.
//   5. [프로젝트] > [새 항목 추가]로 이동하여 새 코드 파일을 만들거나, [프로젝트] > [기존 항목 추가]로 이동하여 기존 코드 파일을 프로젝트에 추가합니다.
//   6. 나중에 이 프로젝트를 다시 열려면 [파일] > [열기] > [프로젝트]로 이동하고 .sln 파일을 선택합니다.
