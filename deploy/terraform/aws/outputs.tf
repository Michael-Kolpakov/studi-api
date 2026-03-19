output "vpc_id" {
  value = aws_vpc.this.id
}

output "api_public_ip" {
  value = aws_instance.api.public_ip
}

output "api_instance_id" {
  value = aws_instance.api.id
}
