#!/bin/bash
#When the free memory very less ,this script to collect CPU/memory usage information and dmessage information. 
#Version 1.0 time:2014-3-11
#Version 2.0 time:2014-12-23

logfile=/tmp/$0.log

check_os_release()
{
  while true
  do
    os_release=$(grep "Red Hat Enterprise Linux Server release" /etc/issue 2>/dev/null)
    os_release_2=$(grep "Red Hat Enterprise Linux Server release" /etc/redhat-release 2>/dev/null)
    if [ "$os_release" ] && [ "$os_release_2" ]
    then
      if echo "$os_release"|grep "release" >/dev/null 2>&1
      then
        os_release=redhat
        echo "$os_release"
      else
        os_release=""
        echo "$os_release"
      fi
      break
    fi
    os_release=$(grep "Aliyun Linux release" /etc/issue 2>/dev/null)
    os_release_2=$(grep "Aliyun Linux release" /etc/aliyun-release 2>/dev/null)
    if [ "$os_release" ] && [ "$os_release_2" ]
    then
      if echo "$os_release"|grep "release" >/dev/null 2>&1
      then
        os_release=aliyun
        echo "$os_release"
      else
        os_release=""
        echo "$os_release"
      fi
      break
    fi
    os_release_2=$(grep "CentOS" /etc/*release 2>/dev/null)
    if [ "$os_release_2" ]
    then
      if echo "$os_release_2"|grep "release" >/dev/null 2>&1
      then
        os_release=centos
        echo "$os_release"
      else
        os_release=""
        echo "$os_release"
      fi
      break
    fi
    os_release=$(grep -i "ubuntu" /etc/issue 2>/dev/null)
    os_release_2=$(grep -i "ubuntu" /etc/lsb-release 2>/dev/null)
    if [ "$os_release" ] && [ "$os_release_2" ]
    then
      if echo "$os_release"|grep "Ubuntu" >/dev/null 2>&1
      then
        os_release=ubuntu
        echo "$os_release"
      else
        os_release=""
        echo "$os_release"
      fi
      break
    fi
    os_release=$(grep -i "debian" /etc/issue 2>/dev/null)
    os_release_2=$(grep -i "debian" /proc/version 2>/dev/null)
    if [ "$os_release" ] && [ "$os_release_2" ]
    then
      if echo "$os_release"|grep "Linux" >/dev/null 2>&1
      then
        os_release=debian
        echo "$os_release"
      else
        os_release=""
        echo "$os_release"
      fi
      break
    fi
    break
    done
}

rhel_fun()
{
  while true
  do
    vm_mem=$(free -m|grep "buffers/cache"|awk '{print $4}')
    cpu=$(top -bn2|grep "Cpu(s)"|awk '{print $8}'|awk -F'%' '{print $1}'|tail -n1)
    check_cpu=$(echo "$cpu <20" |bc)
    if [[ $vm_mem -le 100 ]] || [[ $check_cpu -eq 1  ]]
    then
      echo "======================================================" >>$logfile
      date >>$logfile
      echo "======================================================" >>$logfile
      echo "The memory is too less." >>$logfile
      free -m >>$logfile
      echo "=======================CPU info========================" >>$logfile
      (ps aux|head -1;ps aux|sort -nrk3|grep -v "RSS") >>$logfile
      echo "=======================Memory info=====================" >>$logfile
      (ps aux|head -1;ps aux|sort -nrk6|grep -v "RSS") >>$logfile
      date >>$logfile
      echo "=======================Dmesg info=====================" >>$logfile
      dmesg >>$logfile
      dmesg -c
    fi
    sleep 10
  done
}

debian_fun()
{
  while true
  do
    vm_mem=$(free -m|grep "buffers/cache"|awk '{print $4}')
    cpu=$(top -bn2|grep "Cpu(s)"|awk '{print $5}'|awk -F'%' '{print $1}'|tail -n1)
    check_cpu=$(echo "$cpu <20" |bc)
    if [[ $vm_mem -le 100 ]] || [[ $check_cpu -eq 1  ]]
    then
      echo "======================================================" >>$logfile
      date >>$logfile
      echo "======================================================" >>$logfile
      echo "The memory is too less." >>$logfile
      free -m >>$logfile
      echo "=======================CPU info========================" >>$logfile
      (ps aux|head -1;ps aux|sort -nrk3|grep -v "RSS") >>$logfile
      echo "=======================Memory info=====================" >>$logfile
      (ps aux|head -1;ps aux|sort -nrk6|grep -v "RSS") >>$logfile
      date >>$logfile
      echo "=======================Dmesg info=====================" >>$logfile
      dmesg >>$logfile
      dmesg -c
    fi
    sleep 10
  done
}

check_os_release

case "$os_release" in
redhat|centos|aliyun)
  yum install bc -y
  rhel_fun
  ;;
debian|ubuntu)
  apt-get install bc -y
  debian_fun
  ;;
esac