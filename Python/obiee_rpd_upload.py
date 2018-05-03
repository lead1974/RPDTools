import sys
import os
import configparser
import paramiko
import re

import argparse

import base64

#sys.path.append('../python_getserverinfo/')
import getserverinfo

email_server=getserverinfo.email_server
ip=getserverinfo.obiee_ip
port=getserverinfo.obiee_port
username=getserverinfo.obiee_linux_user
password=base64.b64decode(getserverinfo.obiee_linux_user_pass)

weblogic_pass=base64.b64decode(getserverinfo.obiee_obiee_user_pass)
str_weblogic_pass=str(weblogic_pass)

def connect_obiee(fileName):
    #return args.fileName
    ssh=paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    ssh.connect(ip,port,username,password)
    
    sftp = ssh.open_sftp()
    sftp.put(fileName, getserverinfo.obiee_RPD_path)
    sftp.close()

    cmd ='/u01/Oracle/Middleware/Oracle_Home/user_projects/domains/bi/bitools/bin/data-model-cmd.sh uploadrpd -SI ssi -U '
    cmd+=getserverinfo.obiee_obiee_user
    cmd+=' -p ' 
    cmd+= str_weblogic_pass[1:]
    cmd+= ' -S '
    cmd+= getserverinfo.obiee_server
    cmd+=' -N 9502 -I '
    cmd+= getserverinfo.obiee_RPD_path
    cmd+=' -W '
    cmd+=getserverinfo.obiee_RPD_pass
    #print(cmd)

    stdin,stdout,stderr=ssh.exec_command(cmd)
    errorlines=stderr.readlines()
    outlines=stdout.readlines()
    resp=''.join(errorlines).join(outlines)
    print(resp)

    ssh.close()
    return(resp)

if __name__ == "__main__":
   parser = argparse.ArgumentParser(description='This is a linux command runner for DVO')
   parser.add_argument('-f','--fileName', help='Input File Name to Upload',required=True)
   args = parser.parse_args()
   args_fileName=args.fileName
   filename=getserverinfo.obiee_RPD_win_path #args_fileName
   print(filename)
   connect_obiee(filename)

